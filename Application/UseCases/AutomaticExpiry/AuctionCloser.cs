using Application.Common.Broadcast;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.AutomaticExpiry {
	public class AuctionCloser : IAuctionCloser {

		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuctionRepository _auctionRepository;
		private readonly IBroadcastService _broadcastService;
		private readonly ILogger<AuctionCloser> _logger;
		private readonly IBidRepository _bidRepository;
		private readonly IWalletTransactionRepository _walletTransactionRepository;

		public AuctionCloser(IUnitOfWork unitOfWork,
							 IAuctionRepository auctionRepository,
							 IBroadcastService broadcastService,
							 ILogger<AuctionCloser> logger,
							 IBidRepository bidRepository,
							 IWalletTransactionRepository walletTransactionRepository) {
			_unitOfWork = unitOfWork;
			_auctionRepository = auctionRepository;
			_broadcastService = broadcastService;
			_logger = logger;
			_bidRepository = bidRepository;
			_walletTransactionRepository = walletTransactionRepository;
		}

		public async Task AutomaticClose() {

			// Get current time
			var currentTime = DateTime.UtcNow;

			// Get expired auctions
			var expiredAuctions = await _auctionRepository.GetExpiredAuctionsAsync(currentTime);

			foreach (var expiredAuction in expiredAuctions) {

				if (expiredAuction is null) {
					_logger.LogCritical("Automatic Expiry, null auction found. Time: {Time}", currentTime);
					continue;
				}

				expiredAuction.Status = (int)AuctionStatusEnum.Ended;

				// CASE1: No bids just close auction
				if (expiredAuction.Bids.Any() is false) {

					_ = await _auctionRepository.UpdateAsync(expiredAuction);
					_ = await _unitOfWork.SaveChangesAsync();

					await Broadcast(expiredAuction.Id, Guid.Empty, null);

					continue;
				}

				var transactions = new List<WalletTransaction>();

				// CASE2: Only one bidder
				if (expiredAuction.Bids.Count == 1) {

					var bid = expiredAuction.Bids.FirstOrDefault();
					bid.IsWinningBid = true;

					// Debit bidder
					bid.Bidder.Wallet.FrozenBalance -= bid.Amount;
					bid.Bidder.Wallet.Balance -= bid.Amount;

					// Credit seller
					expiredAuction.Seller.Wallet.Balance += bid.Amount;

					_ = await _bidRepository.UpdateAsync(bid);
					_ = await _auctionRepository.UpdateAsync(expiredAuction);

					// Record transactions
					transactions.AddRange(new[] {
						new WalletTransaction {
							WalletId = bid.Bidder.Wallet.Id,
							BidId = bid.Id,
							Amount = bid.Amount,
							TransactionType = (int)WalletTransactionEnum.Unfreeze
						},
						new WalletTransaction {
							WalletId = bid.Bidder.Wallet.Id,
							BidId = bid.Id,
							Amount = bid.Amount,
							TransactionType = (int)WalletTransactionEnum.Debit
						},
						new WalletTransaction {
							WalletId = expiredAuction.Seller.Wallet.Id,
							Amount = bid.Amount,
							TransactionType = (int)WalletTransactionEnum.Credit
						}
					});

					foreach (var transaction in transactions)
						_ = await _walletTransactionRepository.CreateAsync(transaction);

					_ = await _unitOfWork.SaveChangesAsync();

					await Broadcast(expiredAuction.Id, bid.BidderId, bid.Amount);

					continue;
				}

				// CASE3: Vickrey logic -> Many bidders

				// Order auction bids by amount
				// Order again by date created -> Solve ties on amount -> Earliest wins
				var orderedBids = expiredAuction.Bids.OrderByDescending(x => x.Amount)
												.ThenBy(b => b.DateCreated)
												.ToList();

				var winnerBid = orderedBids.FirstOrDefault();
				winnerBid.IsWinningBid = true;

				var secondAmount = orderedBids.Select(x => x.Amount) // already ordered
											  .Skip(1)
											  .FirstOrDefault();

				// Debit winner
				winnerBid.Bidder.Wallet.FrozenBalance -= winnerBid.Amount;
				winnerBid.Bidder.Wallet.Balance -= secondAmount; // debit second price
				_ = await _bidRepository.UpdateAsync(winnerBid);

				// Credit seller
				expiredAuction.Seller.Wallet.Balance += secondAmount;

				transactions.AddRange(new[] {
					new WalletTransaction {
						WalletId = winnerBid.Bidder.Wallet.Id,
						BidId = winnerBid.Id,
						Amount = winnerBid.Amount,
						TransactionType = (int)WalletTransactionEnum.Unfreeze
					},
					new WalletTransaction {
						WalletId = winnerBid.Bidder.Wallet.Id,
						BidId = winnerBid.Id,
						Amount = secondAmount,
						TransactionType = (int)WalletTransactionEnum.Debit
					},
					new WalletTransaction {
						WalletId = expiredAuction.Seller.Wallet.Id,
						Amount = secondAmount,
						TransactionType = (int)WalletTransactionEnum.Credit
					},
				});

				// Unfreeze all other bidders that did not win
				foreach (var loserBid in expiredAuction.Bids.Where(x => x.IsWinningBid == false)) {

					loserBid.Bidder.Wallet.FrozenBalance -= loserBid.Amount;

					transactions.Add(new WalletTransaction {
						WalletId = loserBid.Bidder.Wallet.Id,
						BidId = loserBid.Id,
						Amount = loserBid.Amount,
						TransactionType = (int)WalletTransactionEnum.Unfreeze
					});

					_ = await _bidRepository.UpdateAsync(loserBid);
				}

				foreach (var transaction in transactions)
					_ = await _walletTransactionRepository.CreateAsync(transaction);

				_ = await _auctionRepository.UpdateAsync(expiredAuction);
				_ = await _unitOfWork.SaveChangesAsync();

				await Broadcast(expiredAuction.Id, winnerBid.BidderId, secondAmount);
			}
		}

		private async Task Broadcast(Guid auctionId, Guid winnerId, decimal? winnerBid) {

			await _broadcastService.PublishAsync("END-AUCTION", new {
				AuctionId = auctionId,
				WinnerId = winnerId,
				WinningBid = winnerBid
			});
		}
	}
}

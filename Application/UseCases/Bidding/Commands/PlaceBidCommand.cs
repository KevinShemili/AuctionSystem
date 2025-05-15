using Application.Common.Broadcast;
using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Bidding.Commands {
	public class PlaceBidCommand : IRequest<Result<Guid>> {
		public Guid AuctionId { get; set; }
		public Guid BidderId { get; set; }
		public decimal Amount { get; set; }
	}

	public class PlaceBidCommandHandler : IRequestHandler<PlaceBidCommand, Result<Guid>> {

		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<PlaceBidCommandHandler> _logger;
		private readonly IAuctionRepository _auctionRepository;
		private readonly IUserRepository _userRepository;
		private readonly IBidRepository _bidRepository;
		private readonly IBroadcastService _broadcastService;
		private readonly IWalletTransactionRepository _walletTransactionRepository;

		// Injecting the dependencies through the constructor.
		public PlaceBidCommandHandler(IUnitOfWork unitOfWork,
									  ILogger<PlaceBidCommandHandler> logger,
									  IUserRepository userRepository,
									  IAuctionRepository auctionRepository,
									  IBidRepository bidRepository,
									  IBroadcastService broadcastService,
									  IWalletTransactionRepository walletTransactionRepository) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_userRepository = userRepository;
			_auctionRepository = auctionRepository;
			_bidRepository = bidRepository;
			_broadcastService = broadcastService;
			_walletTransactionRepository = walletTransactionRepository;
		}

		public async Task<Result<Guid>> Handle(PlaceBidCommand request, CancellationToken cancellationToken) {

			// Get the auction with:
			// 1. Bids -> Bidder -> Wallet -> Transactions
			// 2. Seller
			var auction = await _auctionRepository.GetAuctionWithBidsSellerWalletTransactionsAsync(request.AuctionId, cancellationToken);

			// Check if the auction exists
			if (auction is null) {
				_logger.LogWarning("Place Bid failed: auction not found. AuctionId: {AuctionId}.", request.AuctionId);
				return Result<Guid>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			// Check if the person placing the bid is the seller
			if (auction.Seller.Id == request.BidderId) {
				_logger.LogWarning("Place Bid failed: bidder is seller. AuctionId: {AuctionId}, SellerId: {SellerId}.", request.AuctionId, auction.Seller.Id);
				return Result<Guid>.Failure(Errors.BidderIsSeller);
			}

			// Get the bidder with:
			// 1. Wallet -> Transactions
			var bidder = await _userRepository.GetUserWithWalletAndTransactionsAsync(request.BidderId, cancellationToken);

			// Check if the bidder exists
			if (bidder is null) {
				_logger.LogWarning("Place Bid failed: bidder not found. UserId: {UserId}.", request.BidderId);
				return Result<Guid>.Failure(Errors.Unauthorized);
			}

			// Check if the bidder is an admin
			if (bidder.IsAdministrator is true) {
				_logger.LogWarning("Place Bid failed: bidder is admin. UserId: {UserId}.", request.BidderId);
				return Result<Guid>.Failure(Errors.AdminsCannotPlaceBids);
			}

			// Check if the auction is active
			if (auction.Status != (int)AuctionStatusEnum.Active) {
				_logger.LogWarning("Place Bid failed: auction not active. AuctionId: {AuctionId}, Status: {Status}.", request.AuctionId, auction.Status);
				return Result<Guid>.Failure(Errors.AuctionNotActive);
			}

			// Check if the bid is higher than the baseline price
			if (request.Amount < auction.BaselinePrice) {
				_logger.LogWarning("Place Bid failed: bid below base. AuctionId: {AuctionId}, Amount: {Amount}", request.AuctionId, request.Amount);
				return Result<Guid>.Failure(Errors.BidTooLow);
			}

			// Check if the bidder has enough balance
			var availableBalance = bidder.Wallet.Balance - bidder.Wallet.FrozenBalance;

			// CASE1: Bidder has already bid once -> overide the previous bid
			if (auction.Bids.Any(x => x.BidderId == request.BidderId) is true) {

				// Get previous bid
				var existingBid = auction.Bids.FirstOrDefault(x => x.BidderId == request.BidderId);

				// Calculate difference between the new and existing bid
				var amountDifference = request.Amount - existingBid.Amount;

				// If difference is 0, just return the existing bid ID
				if (amountDifference != 0) {

					// SUBCASE1: Wants to increase the bid
					if (amountDifference > 0) {

						// Check if he has enough balance to cover difference
						if (amountDifference > availableBalance) {
							_logger.LogWarning("Place Bid failed: Insufficient funds to increase bid. AuctionId: {AuctionId}, Amount: {Amount}", request.AuctionId, request.Amount);
							return Result<Guid>.Failure(Errors.IncreaseBidInsufficientFunds);
						}

						// Increase frozen balance
						bidder.Wallet.FrozenBalance += Math.Abs(amountDifference);

						// Add transaction
						var transaction = new WalletTransaction {
							Amount = Math.Abs(amountDifference),
							TransactionType = (int)WalletTransactionEnum.Freeze,
							WalletId = bidder.Wallet.Id,
							BidId = existingBid.Id,
							DateCreated = DateTime.UtcNow
						};

						_ = await _walletTransactionRepository.CreateAsync(transaction, cancellationToken: cancellationToken);
					}

					// SUBCASE2: Wants to decrease the bid
					else {

						// Unfreeze the difference
						bidder.Wallet.FrozenBalance -= Math.Abs(amountDifference);

						// Add transaction
						var transaction = new WalletTransaction {
							Amount = Math.Abs(amountDifference),
							TransactionType = (int)WalletTransactionEnum.Unfreeze,
							WalletId = bidder.Wallet.Id,
							BidId = existingBid.Id,
							DateCreated = DateTime.UtcNow
						};

						_ = await _walletTransactionRepository.CreateAsync(transaction, cancellationToken: cancellationToken);
					}

					// Update the bid
					existingBid.Amount = request.Amount;

					_ = await _bidRepository.UpdateAsync(existingBid, cancellationToken: cancellationToken);

					_ = await _unitOfWork.SaveChangesAsync(cancellationToken);
				}

				return Result<Guid>.Success(existingBid.Id);
			}
			else { // First time bidding

				// Check if the bidder has enough balance to cover the bid amount
				if (availableBalance < request.Amount) {
					_logger.LogWarning("Place Bid failed: insufficient funds. UserId: {UserId}, WalletId: {WalletId}", bidder.Id, bidder.Wallet.Id);
					return Result<Guid>.Failure(Errors.InsufficientFunds);
				}

				// Create a new bid
				var bid = new Bid {
					Amount = request.Amount,
					IsWinningBid = false,
					AuctionId = request.AuctionId,
					BidderId = request.BidderId
				};

				// Create a new transaction
				var transaction = new WalletTransaction {
					Amount = request.Amount,
					TransactionType = (int)WalletTransactionEnum.Freeze,
					WalletId = bidder.Wallet.Id,
					Bid = bid,
					DateCreated = DateTime.UtcNow
				};

				// Freeze the amount in the wallet
				bidder.Wallet.FrozenBalance += request.Amount;
				bidder.Wallet.Transactions.Add(transaction);


				_ = await _bidRepository.CreateAsync(bid, cancellationToken: cancellationToken);

				// Persist changes
				_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

				// Broadcast to client the new bid
				await Broadcast(request.AuctionId, bid.Id, bidder.FirstName, bidder.LastName);

				return Result<Guid>.Success(bid.Id);
			}
		}

		private async Task Broadcast(Guid auctionId, Guid bidId, string firstName, string lastName) {

			await _broadcastService.PublishAsync("NEW-BID", new {
				AuctionId = auctionId,
				BidId = bidId,
				FirstName = firstName,
				LastName = lastName,
				PlacedAt = DateTime.UtcNow
			});
		}
	}

	public class PlaceBidCommandValidator : AbstractValidator<PlaceBidCommand> {
		public PlaceBidCommandValidator() {
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("AuctionId is required.");

			RuleFor(x => x.BidderId)
				.NotEmpty().WithMessage("BidderId is required.");

			RuleFor(x => x.Amount)
				.NotEmpty().WithMessage("Amount is required.");
		}
	}
}

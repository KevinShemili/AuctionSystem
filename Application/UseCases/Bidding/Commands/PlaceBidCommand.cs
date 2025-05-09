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

			var auction = await _auctionRepository.GetAuctionWithBidsSellerWalletTransactionsNoTrackingAsync(request.AuctionId, cancellationToken);

			if (auction is null) {
				_logger.LogWarning("Place Bid failed: auction not found. AuctionId: {AuctionId}.", request.AuctionId);
				return Result<Guid>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			if (auction.Seller.Id == request.BidderId) {
				_logger.LogWarning("Place Bid failed: bidder is seller. AuctionId: {AuctionId}, SellerId: {SellerId}.", request.AuctionId, auction.Seller.Id);
				return Result<Guid>.Failure(Errors.BidderIsSeller);
			}

			var bidder = await _userRepository.GetUserWithWalletAndTransactionsAsync(request.BidderId, cancellationToken);

			if (bidder is null) {
				_logger.LogWarning("Place Bid failed: bidder not found. UserId: {UserId}.", request.BidderId);
				return Result<Guid>.Failure(Errors.Unauthorized);
			}

			if (auction.Status != (int)AuctionStatusEnum.Active) {
				_logger.LogWarning("Place Bid failed: auction not active. AuctionId: {AuctionId}, Status: {Status}.", request.AuctionId, auction.Status);
				return Result<Guid>.Failure(Errors.AuctionNotActive);
			}

			if (request.Amount < auction.BaselinePrice) {
				_logger.LogWarning("Place Bid failed: bid below base. AuctionId: {AuctionId}, Amount: {Amount}", request.AuctionId, request.Amount);
				return Result<Guid>.Failure(Errors.BidTooLow);
			}

			var availableBalance = bidder.Wallet.Balance - bidder.Wallet.FrozenBalance;

			// has already bid once, overide the previous bid
			if (auction.Bids.Any(x => x.BidderId == request.BidderId) is true) {

				var existingBid = auction.Bids.FirstOrDefault(x => x.BidderId == request.BidderId);

				var amountDifference = request.Amount - existingBid.Amount;

				if (amountDifference != 0) {
					if (amountDifference > 0) { // Wants to increase

						if (amountDifference > availableBalance) {
							_logger.LogWarning("Place Bid failed: Insufficient funds to increase bid. AuctionId: {AuctionId}, Amount: {Amount}", request.AuctionId, request.Amount);
							return Result<Guid>.Failure(Errors.IncreaseBidInsufficientFunds);
						}

						bidder.Wallet.FrozenBalance += Math.Abs(amountDifference);

						var transaction = new WalletTransaction {
							Amount = Math.Abs(amountDifference),
							TransactionType = (int)WalletTransactionEnum.Freeze,
							WalletId = bidder.Wallet.Id,
							BidId = existingBid.Id,
							DateCreated = DateTime.UtcNow
						};

						_ = await _walletTransactionRepository.CreateAsync(transaction, cancellationToken: cancellationToken);
					}
					else { // Wants to decrease
						bidder.Wallet.FrozenBalance -= Math.Abs(amountDifference);

						var transaction = new WalletTransaction {
							Amount = Math.Abs(amountDifference),
							TransactionType = (int)WalletTransactionEnum.Unfreeze,
							WalletId = bidder.Wallet.Id,
							BidId = existingBid.Id,
							DateCreated = DateTime.UtcNow
						};

						_ = await _walletTransactionRepository.CreateAsync(transaction, cancellationToken: cancellationToken);
					}

					existingBid.Amount = request.Amount;

					_ = await _bidRepository.UpdateAsync(existingBid, cancellationToken: cancellationToken);

					_ = await _unitOfWork.SaveChangesAsync(cancellationToken);
				}

				return Result<Guid>.Success(existingBid.Id);
			}
			else { // First time bidding

				if (availableBalance < request.Amount) {
					_logger.LogWarning("Place Bid failed: insufficient funds. UserId: {UserId}, WalletId: {WalletId}", bidder.Id, bidder.Wallet.Id);
					return Result<Guid>.Failure(Errors.InsufficientFunds);
				}

				var bid = new Bid {
					Amount = request.Amount,
					IsWinningBid = false,
					AuctionId = request.AuctionId,
					BidderId = request.BidderId
				};

				var transaction = new WalletTransaction {
					Amount = request.Amount,
					TransactionType = (int)WalletTransactionEnum.Freeze,
					WalletId = bidder.Wallet.Id,
					Bid = bid,
					DateCreated = DateTime.UtcNow
				};

				bidder.Wallet.FrozenBalance += request.Amount;
				bidder.Wallet.Transactions.Add(transaction);

				_ = await _bidRepository.CreateAsync(bid, cancellationToken: cancellationToken);

				_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

				await _broadcastService.PublishAsync("NEW-BID", new {
					AuctionId = request.AuctionId,
					BidId = bid.Id,
					FirstName = bidder.FirstName,
					LastName = bidder.LastName,
					PlacedAt = DateTime.UtcNow
				});

				return Result<Guid>.Success(bid.Id);
			}
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

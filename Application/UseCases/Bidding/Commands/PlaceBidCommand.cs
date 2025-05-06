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

		public PlaceBidCommandHandler(IUnitOfWork unitOfWork,
									  ILogger<PlaceBidCommandHandler> logger,
									  IUserRepository userRepository,
									  IAuctionRepository auctionRepository,
									  IBidRepository bidRepository,
									  IBroadcastService broadcastService) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_userRepository = userRepository;
			_auctionRepository = auctionRepository;
			_bidRepository = bidRepository;
			_broadcastService = broadcastService;
		}

		public async Task<Result<Guid>> Handle(PlaceBidCommand request, CancellationToken cancellationToken) {

			var auction = await _auctionRepository.GetAuctionWithBidsNoTrackingAsync(request.AuctionId, cancellationToken);

			if (auction is null) {
				_logger.LogWarning("Place Bid failed: auction not found. AuctionId: {AuctionId}.", request.AuctionId);
				return Result<Guid>.Failure(Errors.AuctionNotFound);
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

			await _unitOfWork.SaveChangesAsync(cancellationToken);

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

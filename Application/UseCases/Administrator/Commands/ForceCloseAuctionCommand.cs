using Application.Common.Broadcast;
using Application.Common.EmailService;
using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Commands {
	public class ForceCloseAuctionCommand : IRequest<Result<bool>> {
		public Guid AuctionId { get; set; }
		public Guid AdminId { get; set; }
		public string Reason { get; set; }
	}

	public class ForceCloseAuctionCommandHandler : IRequestHandler<ForceCloseAuctionCommand, Result<bool>> {

		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ForceCloseAuctionCommandHandler> _logger;
		private readonly IAuctionRepository _auctionRepository;
		private readonly IEmailService _emailService;
		private readonly IBroadcastService _broadcastService;
		private readonly IBidRepository _bidRepository;

		// Injecting the dependencies through the constructor.
		public ForceCloseAuctionCommandHandler(IAuctionRepository auctionRepository,
										 ILogger<ForceCloseAuctionCommandHandler> logger,
										 IUnitOfWork unitOfWork,
										 IBroadcastService broadcastService,
										 IEmailService emailService,
										 IBidRepository bidRepository) {
			_auctionRepository = auctionRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_broadcastService = broadcastService;
			_emailService = emailService;
			_bidRepository = bidRepository;
		}

		public async Task<Result<bool>> Handle(ForceCloseAuctionCommand request, CancellationToken cancellationToken) {

			// Get the auction with:
			// 1. Bids -> Bidder -> Wallet -> Transactions
			// 2. Seller 
			var auction = await _auctionRepository.GetAuctionWithBidsSellerWalletTransactionsAsync(request.AuctionId, cancellationToken);

			// Check if the auction exists
			if (auction == null) {
				_logger.LogWarning("Auction with ID {AuctionId} not found.", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			// Check if the auction is in the active state
			if (auction.Status != (int)AuctionStatusEnum.Active) {
				_logger.LogWarning("Auction with ID {AuctionId} is not active.", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotActive);
			}

			// We need to close the active bids if any.
			if (auction.Bids.Count > 0) {

				// Iterate through the bids.
				foreach (var bid in auction.Bids) {

					var bidder = bid.Bidder; // Get the bidder
					bidder.Wallet.FrozenBalance -= bid.Amount; // Unfreeze his balance

					// Create a new transaction to illustrate the unfreeze
					bidder.Wallet.Transactions.Add(new WalletTransaction {
						Amount = bid.Amount,
						TransactionType = (int)WalletTransactionEnum.Unfreeze,
						DateCreated = DateTime.UtcNow
					});

					// Delete the bid
					_ = await _bidRepository.DeleteAsync(bid, cancellationToken: cancellationToken);
					// Notify bidder that his bid has been removed
					await _emailService.SendBidRemovedEmailAsync(bidder.Email, auction.Name, cancellationToken);
				}
			}

			// Close the auction & state reason
			auction.Status = (int)AuctionStatusEnum.Ended;
			auction.ForceClosedBy = request.AdminId;
			auction.ForceClosedReason = request.Reason;

			// Update & Persist the auction
			_ = await _auctionRepository.UpdateAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			// Notify seller that his auction has been closed
			await _emailService.SendAuctionClosedEmailAsync(auction.Seller.Email, auction.Name, auction.Seller.LastName, request.Reason, cancellationToken);

			// Broadcast to frontend that the auction has ended
			await Broadcast(auction.Id, Guid.Empty, null);

			return Result<bool>.Success(true);
		}

		private async Task Broadcast(Guid auctionId, Guid winnerId, decimal? winnerBid) {

			await _broadcastService.PublishAsync("END-AUCTION", new {
				AuctionId = auctionId,
				WinnerId = winnerId,
				WinningBid = winnerBid
			});
		}
	}

	public class ForceCloseAuctionCommandValidator : AbstractValidator<ForceCloseAuctionCommand> {
		public ForceCloseAuctionCommandValidator() {
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("Auction ID is required.");
			RuleFor(x => x.AdminId)
				.NotEmpty().WithMessage("Admin ID is required.");
			RuleFor(x => x.Reason)
				.NotEmpty().WithMessage("Reason is required.");
		}
	}

}
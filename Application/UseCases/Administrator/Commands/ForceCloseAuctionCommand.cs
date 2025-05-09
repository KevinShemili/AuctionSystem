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

			var auction = await _auctionRepository.GetAuctionWithBidsSellerWalletTransactionsNoTrackingAsync(request.AuctionId, cancellationToken);

			if (auction == null) {
				_logger.LogWarning("Auction with ID {AuctionId} not found.", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			if (auction.Status != (int)AuctionStatusEnum.Active) {
				_logger.LogWarning("Auction with ID {AuctionId} is not active.", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotActive);
			}

			if (auction.Bids.Count > 0) {

				foreach (var bid in auction.Bids) {

					var bidder = bid.Bidder;
					bidder.Wallet.FrozenBalance -= bid.Amount;
					bidder.Wallet.Transactions.Add(new WalletTransaction {
						Amount = bid.Amount,
						TransactionType = (int)WalletTransactionEnum.Credit,
						DateCreated = DateTime.UtcNow,
						BidId = bid.Id
					});

					_ = await _bidRepository.DeleteAsync(bid, cancellationToken: cancellationToken);
					await _emailService.SendBidRemovedEmailAsync(bidder.Email, auction.Name, cancellationToken);
				}
			}

			auction.Status = (int)AuctionStatusEnum.Ended;
			auction.ForceClosedBy = request.AdminId;
			auction.ForceClosedReason = request.Reason;

			_ = await _auctionRepository.UpdateAsync(auction, cancellationToken: cancellationToken);

			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			await _emailService.SendAuctionClosedEmailAsync(auction.Seller.Email, auction.Name, auction.Seller.LastName, request.Reason, cancellationToken);
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
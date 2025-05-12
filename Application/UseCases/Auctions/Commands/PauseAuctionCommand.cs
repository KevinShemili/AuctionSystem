using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auctions.Commands {
	public class PauseAuctionCommand : IRequest<Result<bool>> {
		public Guid UserId { get; set; }
		public Guid AuctionId { get; set; }
	}

	public class PauseAuctionCommandHandler : IRequestHandler<PauseAuctionCommand, Result<bool>> {

		private readonly IAuctionRepository _auctionRepository;
		private readonly ILogger<PauseAuctionCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		// Injecting the dependencies through the constructor.
		public PauseAuctionCommandHandler(IAuctionRepository auctionRepository,
									IUnitOfWork unitOfWork,
									ILogger<PauseAuctionCommandHandler> logger) {
			_auctionRepository = auctionRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Result<bool>> Handle(PauseAuctionCommand request, CancellationToken cancellationToken) {

			// Get the auction with:
			// 1. Bids
			var auction = await _auctionRepository.GetAuctionWithBidsAsync(request.AuctionId, cancellationToken);

			// Check if the auction exists
			if (auction is null) {
				_logger.LogWarning("Auction with id {AuctionId} not found", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			// Check the action is performed by the seller
			if (auction.SellerId != request.UserId) {
				return Result<bool>.Failure(Errors.ChangeOnlyOwnAuctions);
			}

			// Check if the auction is in the active state
			if (auction.Status != (int)AuctionStatusEnum.Active) {
				return Result<bool>.Failure(Errors.AuctionNotActive);
			}

			// Check if the auction has bids
			if (auction.Bids.Any() is true) {
				return Result<bool>.Failure(Errors.AuctionHasBids);
			}

			// Pause the auction
			auction.Status = (int)AuctionStatusEnum.Paused;

			// Update & Persist
			_ = await _auctionRepository.UpdateAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class PauseAuctionCommandValidator : AbstractValidator<PauseAuctionCommand> {
		public PauseAuctionCommandValidator() {
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("UserId is required");
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("AuctionId is required");
		}
	}
}
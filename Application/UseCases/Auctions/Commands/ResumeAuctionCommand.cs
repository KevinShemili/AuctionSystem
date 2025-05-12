using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.Tools.Time;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auctions.Commands {
	public class ResumeAuctionCommand : IRequest<Result<bool>> {
		public Guid UserId { get; set; }
		public Guid AuctionId { get; set; }
		public DateTime EndTime { get; set; }
	}

	public class ResumeAuctionCommandHandler : IRequestHandler<ResumeAuctionCommand, Result<bool>> {

		private readonly IAuctionRepository _auctionRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ResumeAuctionCommandHandler> _logger;

		// INjecting the dependencies through the constructor.
		public ResumeAuctionCommandHandler(IAuctionRepository auctionRepository,
									IUnitOfWork unitOfWork,
									ILogger<ResumeAuctionCommandHandler> logger) {
			_auctionRepository = auctionRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}
		public async Task<Result<bool>> Handle(ResumeAuctionCommand request, CancellationToken cancellationToken) {

			// Get the auction by ID
			var auction = await _auctionRepository.GetByIdAsync(request.AuctionId, cancellationToken);

			// Check if the auction exists
			if (auction is null) {
				_logger.LogWarning("Auction with id {AuctionId} not found", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			// Check that the user performing the action is the owner of the auction
			if (auction.SellerId != request.UserId) {
				_logger.LogWarning("User {UserId} tried to resume auction {AuctionId} that they do not own", request.UserId, request.AuctionId);
				return Result<bool>.Failure(Errors.ChangeOnlyOwnAuctions);
			}

			// Check if the auction is in the paused state
			if (auction.Status != (int)AuctionStatusEnum.Paused) {
				_logger.LogWarning("Auction with id {AuctionId} is not paused", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotPaused);
			}

			// Set new starting time as the current time, and apply the new end time
			var startTime = TruncateTime.ToMinute(DateTime.UtcNow);
			request.EndTime = TruncateTime.ToMinute(request.EndTime);

			// Check if the end time is in the past
			if (request.EndTime <= startTime) {
				_logger.LogWarning("Create Auction attempt failed, invalid end time. StartTime: {StartTime} EndTime: {EndTime}", startTime, request.EndTime);
				return Result<bool>.Failure(Errors.EndSmallerEqualStart);
			}

			// Change the status and the times.
			auction.Status = (int)AuctionStatusEnum.Active;
			auction.StartTime = startTime;
			auction.EndTime = request.EndTime;

			// Update & Persist
			_ = await _auctionRepository.UpdateAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class ResumeAuctionCommandValidator : AbstractValidator<ResumeAuctionCommand> {
		public ResumeAuctionCommandValidator() {
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("AuctionId is required");
			RuleFor(x => x.EndTime)
				.NotEmpty().WithMessage("EndTime is required");
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("UserId is required");
		}
	}
}

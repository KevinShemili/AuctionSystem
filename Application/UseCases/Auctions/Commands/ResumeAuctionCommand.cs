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
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}

	public class ResumeAuctionCommandHandler : IRequestHandler<ResumeAuctionCommand, Result<bool>> {

		private readonly IAuctionRepository _auctionRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ResumeAuctionCommandHandler> _logger;

		public ResumeAuctionCommandHandler(IAuctionRepository auctionRepository,
									IUnitOfWork unitOfWork,
									ILogger<ResumeAuctionCommandHandler> logger) {
			_auctionRepository = auctionRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}
		public async Task<Result<bool>> Handle(ResumeAuctionCommand request, CancellationToken cancellationToken) {

			var auction = await _auctionRepository.GetByIdAsync(request.AuctionId, cancellationToken);

			if (auction is null) {
				_logger.LogWarning("Auction with id {AuctionId} not found", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			if (auction.SellerId != request.UserId) {
				_logger.LogWarning("User {UserId} tried to resume auction {AuctionId} that they do not own", request.UserId, request.AuctionId);
				return Result<bool>.Failure(Errors.ChangeOnlyOwnAuctions);
			}

			if (auction.Status != (int)AuctionStatusEnum.Paused) {
				_logger.LogWarning("Auction with id {AuctionId} is not paused", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotPaused);
			}

			request.StartTime = TruncateTime.ToMinute(request.StartTime);
			request.EndTime = TruncateTime.ToMinute(request.EndTime);

			if (request.StartTime < DateTime.UtcNow) {
				_logger.LogWarning("Create Auction attempt failed, past start time. TimeNow: {TimeNow} StartTime: {StartTime}", DateTime.UtcNow, request.StartTime);
				return Result<bool>.Failure(Errors.PastStartTime);
			}

			if (request.EndTime <= request.StartTime) {
				_logger.LogWarning("Create Auction attempt failed, invalid end time. StartTime: {StartTime} EndTime: {EndTime}", request.StartTime, request.EndTime);
				return Result<bool>.Failure(Errors.EndSmallerEqualStart);
			}

			auction.Status = (int)AuctionStatusEnum.Active;
			auction.StartTime = request.StartTime;
			auction.EndTime = request.EndTime;

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

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

		public PauseAuctionCommandHandler(IAuctionRepository auctionRepository,
									IUnitOfWork unitOfWork,
									ILogger<PauseAuctionCommandHandler> logger) {
			_auctionRepository = auctionRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Result<bool>> Handle(PauseAuctionCommand request, CancellationToken cancellationToken) {

			var auction = await _auctionRepository.GetAuctionWithBidsAsync(request.AuctionId, cancellationToken);

			if (auction is null) {
				_logger.LogWarning("Auction with id {AuctionId} not found", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			if (auction.SellerId != request.UserId) {
				return Result<bool>.Failure(Errors.ChangeOnlyOwnAuctions);
			}

			if (auction.Status != (int)AuctionStatusEnum.Active) {
				return Result<bool>.Failure(Errors.AuctionNotActive);
			}

			if (auction.Bids.Any() is true) {
				return Result<bool>.Failure(Errors.AuctionHasBids);
			}

			auction.Status = (int)AuctionStatusEnum.Paused;
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
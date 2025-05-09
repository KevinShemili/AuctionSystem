using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Commands {
	public class ForceDeleteAuctionCommand : IRequest<Result<bool>> {
		public Guid AuctionId { get; set; }
	}

	public class ForceDeleteAuctionCommandHandler : IRequestHandler<ForceDeleteAuctionCommand, Result<bool>> {

		private readonly IUnitOfWork _unitOfWork;
		private readonly IAuctionRepository _auctionRepository;
		private readonly ILogger<ForceDeleteAuctionCommandHandler> _logger;

		public ForceDeleteAuctionCommandHandler(IUnitOfWork unitOfWork,
											   IAuctionRepository auctionRepository,
											   ILogger<ForceDeleteAuctionCommandHandler> logger) {
			_unitOfWork = unitOfWork;
			_auctionRepository = auctionRepository;
			_logger = logger;
		}

		public async Task<Result<bool>> Handle(ForceDeleteAuctionCommand request, CancellationToken cancellationToken) {

			var auction = await _auctionRepository.GetByIdAsync(request.AuctionId, cancellationToken);

			if (auction == null) {
				_logger.LogWarning("Auction with ID {AuctionId} not found.", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			if (auction.ForceClosedBy is null || auction.Status != (int)AuctionStatusEnum.Ended) {
				_logger.LogWarning("Auction not yet eneded. AuctionId: {AuctionId}.", request.AuctionId);
				return Result<bool>.Failure(Errors.EndBeforeDelete);
			}

			_ = await _auctionRepository.DeleteAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class ForceDeleteAuctionCommandValidator : AbstractValidator<ForceDeleteAuctionCommand> {
		public ForceDeleteAuctionCommandValidator() {
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("Auction ID cannot be empty.");
		}
	}
}

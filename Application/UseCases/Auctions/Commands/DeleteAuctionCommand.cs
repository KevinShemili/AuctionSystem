﻿using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auctions.Commands {

	public class DeleteAuctionCommand : IRequest<Result<bool>> {
		public Guid UserId { get; set; }
		public Guid AuctionId { get; set; }
	}

	public class DeleteAuctionCommandHandler : IRequestHandler<DeleteAuctionCommand, Result<bool>> {

		private readonly IAuctionRepository _auctionRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<DeleteAuctionCommandHandler> _logger;

		// Injecting the dependencies through the constructor.
		public DeleteAuctionCommandHandler(IAuctionRepository auctionRepository,
										IUnitOfWork unitOfWork,
										ILogger<DeleteAuctionCommandHandler> logger) {
			_auctionRepository = auctionRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Result<bool>> Handle(DeleteAuctionCommand request, CancellationToken cancellationToken) {

			// Get the auction by ID
			var auction = await _auctionRepository.GetByIdAsync(request.AuctionId, cancellationToken);

			// Check if the auction exists
			if (auction is null) {
				_logger.LogWarning("Auction with id {AuctionId} not found", request.AuctionId);
				return Result<bool>.Failure(Errors.AuctionNotFound(request.AuctionId));
			}

			// Check that the user performing the operation is the owner of the auction
			if (auction.SellerId != request.UserId) {
				_logger.LogWarning("User with id {UserId} tried to delete auction with id {AuctionId} that he does not own", request.UserId, request.AuctionId);
				return Result<bool>.Failure(Errors.ChangeOnlyOwnAuctions);
			}

			// Check if the auction is in the paused state
			if (auction.Status != (int)AuctionStatusEnum.Paused) {
				return Result<bool>.Failure(Errors.AuctionNotPaused);
			}

			// Delete & Persist
			_ = await _auctionRepository.DeleteAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class DeleteAuctionCommandValidator : AbstractValidator<DeleteAuctionCommand> {
		public DeleteAuctionCommandValidator() {
			RuleFor(x => x.AuctionId)
				.NotEmpty().WithMessage("AuctionId is required");
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("AuctionId is required");
		}
	}
}

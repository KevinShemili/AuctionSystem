﻿using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.Tools.Time;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Auctions.Commands {
	public class CreateAuctionCommand : IRequest<Result<Guid>> {
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime EndTime { get; set; }
		public IEnumerable<string> Images { get; set; }
		public Guid SellerId { get; set; }
	}

	public class CreateAuctionCommandHandler : IRequestHandler<CreateAuctionCommand, Result<Guid>> {

		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<CreateAuctionCommandHandler> _logger;
		private readonly IAuctionRepository _auctionRepository;
		private readonly IUserRepository _userRepository;

		// Injecting the dependencies through the constructor.
		public CreateAuctionCommandHandler(IUnitOfWork unitOfWork,
										   ILogger<CreateAuctionCommandHandler> logger,
										   IAuctionRepository auctionRepository,
										   IUserRepository userRepository) {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_auctionRepository = auctionRepository;
			_userRepository = userRepository;
		}

		public async Task<Result<Guid>> Handle(CreateAuctionCommand request, CancellationToken cancellationToken) {

			// Get user by ID
			var user = await _userRepository.GetByIdNoTrackingAsync(request.SellerId, cancellationToken);

			// Check if the user exists
			if (user is null) {
				_logger.LogWarning("Create Auction attempt failed, unauthorized. User: {User}.", user);
				return Result<Guid>.Failure(Errors.Unauthorized);
			}

			// Check if the user is an administrator
			if (user.IsAdministrator) {
				return Result<Guid>.Failure(Errors.NotAccessibleByAdmins);
			}

			// Check if baseline price is valid
			if (request.BaselinePrice <= 0) {
				_logger.LogWarning("Create Auction attempt failed, negative price. BaselinePrice: {BaselinePrice}.", request.BaselinePrice);
				return Result<Guid>.Failure(Errors.NegativeBaselinePrice);
			}

			// Normalize to minute precision
			// Since our Hangfire will trigger every minute we need to make sure the start time is normalized to minute precision.
			var startTime = TruncateTime.ToMinute(DateTime.UtcNow);
			request.EndTime = TruncateTime.ToMinute(request.EndTime);

			// Check if the end time is smaller than the start time
			if (request.EndTime <= startTime) {
				_logger.LogWarning("Create Auction attempt failed, invalid end time. StartTime: {StartTime} EndTime: {EndTime}", startTime, request.EndTime);
				return Result<Guid>.Failure(Errors.EndSmallerEqualStart);
			}

			// Check if there is at least one image
			if (request.Images == null || !request.Images.Any()) {
				_logger.LogWarning("Create Auction attempt failed, no images provided.");
				return Result<Guid>.Failure(Errors.OneOrMoreImages);
			}

			foreach (var img in request.Images) {
				if (img == null || img.Length == 0) {
					_logger.LogWarning("Create Auction attempt failed, empty image data {data}", img);
					return Result<Guid>.Failure(Errors.OneOrMoreImages);
				}
			}

			// Map from request to domain entity
			var auction = Map(request, startTime);

			// Create & Persist
			_ = await _auctionRepository.CreateAsync(auction, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<Guid>.Success(auction.Id);
		}

		private static Auction Map(CreateAuctionCommand request, DateTime startTime) {

			var auction = new Auction {
				Name = request.Name,
				Description = request.Description,
				BaselinePrice = request.BaselinePrice,
				StartTime = startTime,
				EndTime = request.EndTime,
				SellerId = request.SellerId,
				Images = new List<AuctionImage>(),
				Status = (int)AuctionStatusEnum.Active
			};

			foreach (var img in request.Images) {
				auction.Images.Add(new AuctionImage {
					FilePath = img,
					DateCreated = DateTime.UtcNow,
				});
			}

			return auction;
		}
	}

	public class CreateAuctionCommandValidator : AbstractValidator<CreateAuctionCommand> {
		public CreateAuctionCommandValidator() {
			RuleFor(x => x.Name)
				.NotEmpty().WithMessage("Auction title is required.");
			RuleFor(x => x.BaselinePrice)
				.NotEmpty().WithMessage("Baseline price is required.");
			RuleFor(x => x.EndTime)
				.NotEmpty().WithMessage("End time is required.");
		}
	}
}
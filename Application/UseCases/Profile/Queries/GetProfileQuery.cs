﻿using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Profile.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profile.Queries {
	public class GetProfileQuery : IRequest<Result<ProfileDTO>> {
		public Guid UserId { get; set; }
	}

	public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileDTO>> {

		private readonly IUserRepository _userRepository;
		private readonly ILogger<GetProfileQueryHandler> _logger;

		// Injecting the dependencies through the constructor.
		public GetProfileQueryHandler(ILogger<GetProfileQueryHandler> logger, IUserRepository userRepository) {
			_logger = logger;
			_userRepository = userRepository;
		}

		public async Task<Result<ProfileDTO>> Handle(GetProfileQuery request, CancellationToken cancellationToken) {

			// Get the user with:
			// 1. Auctions
			// 2. Wallet
			var user = await _userRepository.GetUserWithAuctionWalletNoTrackingAsync(request.UserId, cancellationToken);

			// Check if the user exists
			if (user is null) {
				_logger.LogCritical("User bypassed authorization. {UserId} not found", request.UserId);
				return Result<ProfileDTO>.Failure(Errors.Unauthorized);
			}

			// Check if the user is an administrator
			if (user.IsAdministrator is true) {
				_logger.LogWarning("User is an administrator. {UserId} is not allowed to access this endpoint", request.UserId);
				return Result<ProfileDTO>.Failure(Errors.NotAccessibleByAdmins);
			}

			// Map to DTO
			return Result<ProfileDTO>.Success(MapResponse(user));
		}

		private static ProfileDTO MapResponse(User user) {

			var profile = new ProfileDTO {
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				WalletId = user.Wallet.Id,
				Balance = user.Wallet.Balance,
				FrozenBalance = user.Wallet.FrozenBalance,
				OwnAuctions = user.Auctions.Select(a => new OwnAuctionsDTO {
					Id = a.Id,
					Name = a.Name,
					BaselinePrice = a.BaselinePrice,
					StartTime = a.StartTime,
					EndTime = a.EndTime,
					Status = a.Status
				}).ToList()
			};

			return profile;
		}
	}
}

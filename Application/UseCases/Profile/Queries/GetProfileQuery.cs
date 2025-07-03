using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using Application.UseCases.Bidding.DTOs;
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
			// 3. Bids
			// 4. Roles -> Permissions -> if Admin
			var user = await _userRepository.GetUserWithAuctionWalletBidsRolesPermissionsNoTrackingAsync(request.UserId, cancellationToken);

			// Check if the user exists
			if (user is null) {
				_logger.LogCritical("User bypassed authorization. {UserId} not found", request.UserId);
				return Result<ProfileDTO>.Failure(Errors.Unauthorized);
			}

			// Map to DTO
			return Result<ProfileDTO>.Success(MapResponse(user));
		}

		private static ProfileDTO MapResponse(User user) {

			var profile = user.IsAdministrator ?
			new ProfileDTO {
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				IsAdmin = user.IsAdministrator,
				Roles = user.Roles.Select(role => new RoleDTO {
					Id = role.Id,
					Name = role.Name,
					Description = role.Description,
					Permissions = role.Permissions.Select(permission => new PermissionDTO {
						Id = permission.Id,
						Key = permission.Key,
						Name = permission.Name,
						Description = permission.Description
					}).ToList()
				}).ToList()
			} :
			new ProfileDTO {
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				WalletId = user.Wallet.Id,
				Balance = user.Wallet.Balance,
				FrozenBalance = user.Wallet.FrozenBalance,
				OwnAuctions = user.Auctions.Select(x => new OwnAuctionsDTO {
					Id = x.Id,
					Name = x.Name,
					BaselinePrice = x.BaselinePrice,
					StartTime = x.StartTime,
					EndTime = x.EndTime,
					Status = x.Status
				}).ToList(),
				PlacedBids = user.Bids.Select(x => new BidDTO {
					Id = x.Id,
					AuctionId = x.Auction.Id,
					Amount = x.Amount,
					IsWinningBid = x.IsWinningBid
				}).ToList()
			};

			return profile;
		}
	}
}
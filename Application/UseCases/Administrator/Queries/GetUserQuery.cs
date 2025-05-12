using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using Application.UseCases.Bidding.DTOs;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Queries {
	public class GetUserQuery : IRequest<Result<UserDetailsDTO>> {
		public Guid UserId { get; set; }
	}

	public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDetailsDTO>> {

		private readonly IUserRepository _userRepository;
		private readonly ILogger<GetUserQueryHandler> _logger;

		public GetUserQueryHandler(IUserRepository userRepository,
							 ILogger<GetUserQueryHandler> logger) {
			_userRepository = userRepository;
			_logger = logger;
		}

		public async Task<Result<UserDetailsDTO>> Handle(GetUserQuery request, CancellationToken cancellationToken) {

			// Get user with:
			// 1. Roles -> Permissions
			// 2. Auctions -> Bids & Images
			// 3. Wallet -> Transactions
			// 4. Bids -> Auction -> Images
			var user = await _userRepository.GetUserWithRolesPermissionsWalletAuctionsBidsNoTrackingAsync(request.UserId, cancellationToken);

			// Check if the user exists
			if (user == null) {
				_logger.LogWarning("User with ID {UserId} not found.", request.UserId);
				return Result<UserDetailsDTO>.Failure(Errors.UserNotFound(request.UserId));
			}

			// Get auctions created by the user
			var createdAuctions = user.Auctions.ToList();
			// Get auctions participated by the user
			var participatedAuctions = user.Bids.Select(x => x.Auction).ToList();

			// Return result mapped to DTO
			return Result<UserDetailsDTO>.Success(MapResponse(user, createdAuctions, participatedAuctions));
		}

		private static UserDetailsDTO MapResponse(User user, IEnumerable<Auction> createdAuctions, IEnumerable<Auction> participatedAuctions) {

			var profile = new UserDetailsDTO {
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				IsAdministrator = user.IsAdministrator,
				IsBlocked = user.IsBlocked,
				WalletId = user.IsAdministrator ? null : user.Wallet.Id,
				Balance = user.IsAdministrator ? null : user.Wallet.Balance,
				FrozenBalance = user.IsAdministrator ? null : user.Wallet.FrozenBalance,
				Roles = user.IsAdministrator ? user.Roles.Select(x => new RoleDTO {
					Id = x.Id,
					Name = x.Name,
					Description = x.Description,
					Permissions = x.Permissions.Select(x => new PermissionDTO {
						Id = x.Id,
						Name = x.Name,
						Description = x.Description
					})
				}) : null,
				CreatedAuctions = user.IsAdministrator ? null : createdAuctions.Select(x => new CreatedAuctionDTO {
					Id = x.Id,
					Name = x.Name,
					BaselinePrice = x.BaselinePrice,
					StartTime = x.StartTime,
					Description = x.Description,
					EndTime = x.EndTime,
					Status = x.Status,
					Images = x.Images.Select(x => x.FilePath),
					Bids = x.Bids.Select(x => new BidDTO {
						Id = x.Id,
						AuctionId = x.AuctionId,
						Amount = x.Amount,
						IsWinningBid = x.IsWinningBid
					})
				}),
				ParticipatedAuctions = user.IsAdministrator ? null : participatedAuctions.Select(x => new PartecipatedAuctionDTO {
					Id = x.Id,
					Name = x.Name,
					BaselinePrice = x.BaselinePrice,
					StartTime = x.StartTime,
					Description = x.Description,
					EndTime = x.EndTime,
					Status = x.Status,
					Images = x.Images.Select(x => x.FilePath),
					BidId = x.Bids.FirstOrDefault(x => x.BidderId == user.Id).Id,
					BidAmount = x.Bids.FirstOrDefault(x => x.BidderId == user.Id).Amount,
					IsWinningBid = x.Bids.FirstOrDefault(x => x.BidderId == user.Id).IsWinningBid
				})
			};

			return profile;
		}
	}

	public class GetUserQueryValidator : AbstractValidator<GetUserQuery> {
		public GetUserQueryValidator() {
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("UserId cannot be empty.");
		}
	}
}

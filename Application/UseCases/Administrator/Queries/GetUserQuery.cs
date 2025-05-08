using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using Application.UseCases.Auctions.DTOs;
using Application.UseCases.Bidding.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Queries {
	public class GetUserQuery : IRequest<Result<UserDTO>> {
		public Guid UserId { get; set; }
	}

	public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDTO>> {

		private readonly IUserRepository _userRepository;
		private readonly ILogger<GetUserQueryHandler> _logger;
		private readonly IAuctionRepository _auctionRepository;

		public GetUserQueryHandler(IUserRepository userRepository, ILogger<GetUserQueryHandler> logger, IAuctionRepository auctionRepository) {
			_userRepository = userRepository;
			_logger = logger;
			_auctionRepository = auctionRepository;
		}

		public async Task<Result<UserDTO>> Handle(GetUserQuery request, CancellationToken cancellationToken) {

			if (request.UserId == Guid.Empty) {
				_logger.LogError("UserId is empty. Request: {Request}", request);
				return Result<UserDTO>.Failure(Errors.InvalidId);
			}

			var user = await _userRepository.GetUserWithRolesPermissionsWalletAuctionsBidsNoTrackingAsync(request.UserId, cancellationToken);

			var createdAuctions = user.Auctions.ToList();
			var participatedAuctions = user.Bids.Select(x => x.Auction).ToList();

			return Result<UserDTO>.Success(MapResponse(user, createdAuctions, participatedAuctions));
		}

		private static UserDTO MapResponse(User user, IEnumerable<Auction> createdAuctions, IEnumerable<Auction> participatedAuctions) {

			var profile = new UserDTO {
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
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
				CreatedAuctions = user.IsAdministrator ? null : createdAuctions.Select(x => new AuctionDTO {
					Id = x.Id,
					Name = x.Name,
					BaselinePrice = x.BaselinePrice,
					StartTime = x.StartTime,
					Description = x.Description,
					EndTime = x.EndTime,
					Status = x.Status,
					Images = x.Images.Select(x => x.FilePath)
				}),
				ParticipatedAuctions = user.IsAdministrator ? null : participatedAuctions.Select(x => new AuctionDTO {
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
						Amount = x.Amount,
						IsWinningBid = x.IsWinningBid
					})
				})
			};

			return profile;
		}
	}
}

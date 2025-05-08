using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using Application.UseCases.Auctions.DTOs;
using Application.UseCases.Bidding.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Administrator.Queries {
	public class GetAllUsersQuery : IRequest<Result<PagedResponse<UserDTO>>> {
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string Filter { get; set; }
		public string SortBy { get; set; }
		public bool SortDesc { get; set; }
	}

	public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, Result<PagedResponse<UserDTO>>> {

		private readonly IUserRepository _userRepository;

		public GetAllUsersQueryHandler(IUserRepository userRepository) {
			_userRepository = userRepository;
		}

		public async Task<Result<PagedResponse<UserDTO>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken) {

			var pagedUsers = await _userRepository.GetAllWithRolesPermissionsWalletAuctionsBidsNoTrackingAsync()
												  .ToPagedResponseAsync(request.Filter, request.PageNumber, request.PageSize,
																		request.SortBy, request.SortDesc);

			var pagedDTO = Map(pagedUsers);

			return Result<PagedResponse<UserDTO>>.Success(pagedDTO);
		}

		private static PagedResponse<UserDTO> Map(PagedResponse<User> pagedUsers) {

			var pagedDTO = new PagedResponse<UserDTO> {
				PageNumber = pagedUsers.PageNumber,
				PageSize = pagedUsers.PageSize,
				TotalRecords = pagedUsers.TotalRecords,
				Items = new List<UserDTO>()
			};

			foreach (var user in pagedUsers.Items) {

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
					CreatedAuctions = user.IsAdministrator ? null : user.Auctions.Select(x => new AuctionDTO {
						Id = x.Id,
						Name = x.Name,
						BaselinePrice = x.BaselinePrice,
						StartTime = x.StartTime,
						Description = x.Description,
						EndTime = x.EndTime,
						Status = x.Status,
						Images = x.Images.Select(x => x.FilePath)
					}),
					ParticipatedAuctions = user.IsAdministrator ? null : user.Bids.Select(x => x.Auction).Select(x => new AuctionDTO {
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

				pagedDTO.Items.Add(profile);
			}

			return pagedDTO;
		}
	}
}

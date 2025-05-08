using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.Contracts.Repositories;
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

			var pagedUsers = await _userRepository.GetAllUsersWithRole()
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
				pagedDTO.Items.Add(new UserDTO {
					Id = user.Id,
					FirstName = user.FirstName,
					LastName = user.LastName,
					Email = user.Email,
					Roles = user.Roles.Select(x => new RoleDTO {
						Id = x.Id,
						Name = x.Name
					}).ToList()
				});
			}

			return pagedDTO;
		}
	}
}

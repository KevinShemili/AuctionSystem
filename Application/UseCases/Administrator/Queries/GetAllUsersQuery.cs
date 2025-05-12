using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
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

		// Injecting the dependencies through the constructor.
		public GetAllUsersQueryHandler(IUserRepository userRepository) {
			_userRepository = userRepository;
		}

		public async Task<Result<PagedResponse<UserDTO>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken) {

			// Get all the users without tracking, apply pagination based on the received parameters
			var pagedUsers = await _userRepository.SetNoTracking()
												  .ToPagedResponseAsync(request.Filter, request.PageNumber, request.PageSize,
																		request.SortBy, request.SortDesc);

			// Map domain entity to DTO
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
					IsAdministrator = user.IsAdministrator,
					IsBlocked = user.IsBlocked
				};

				pagedDTO.Items.Add(profile);
			}

			return pagedDTO;
		}
	}
}

using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Administrator.Queries {
	public class ViewRolesQuery : IRequest<Result<PagedResponse<AllRolesDTO>>> {
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string Filter { get; set; }
		public string SortBy { get; set; }
		public bool SortDesc { get; set; }
	}

	public class ViewRolesQueryHandler : IRequestHandler<ViewRolesQuery, Result<PagedResponse<AllRolesDTO>>> {

		private readonly IRoleRepository _roleRepository;

		// Injecting the dependencies through the constructor.
		public ViewRolesQueryHandler(IRoleRepository roleRepository) {
			_roleRepository = roleRepository;
		}

		public async Task<Result<PagedResponse<AllRolesDTO>>> Handle(ViewRolesQuery request, CancellationToken cancellationToken) {

			// Get all the roles without tracking, apply pagination based on the received parameters
			var pagedRoles = await _roleRepository.SetNoTracking(request.Filter)
												  .ToPagedResponseAsync(request.PageNumber, request.PageSize, request.SortBy, request.SortDesc);

			// Map domain entity to DTO
			var pagedDTO = Map(pagedRoles);

			return Result<PagedResponse<AllRolesDTO>>.Success(pagedDTO);
		}
		private static PagedResponse<AllRolesDTO> Map(PagedResponse<Role> pagedRoles) {

			var pagedDTO = new PagedResponse<AllRolesDTO> {
				PageNumber = pagedRoles.PageNumber,
				PageSize = pagedRoles.PageSize,
				TotalRecords = pagedRoles.TotalRecords,
				Items = pagedRoles.Items.Select(role => new AllRolesDTO {
					Id = role.Id,
					Name = role.Name,
					Description = role.Description,
				}).ToList()
			};

			return pagedDTO;
		}
	}
}
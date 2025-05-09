using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using Domain.Entities;
using MediatR;

namespace Application.UseCases.Administrator.Queries {
	public class ViewPermissionsQuery : IRequest<Result<PagedResponse<PermissionDTO>>> {
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public string Filter { get; set; }
		public string SortBy { get; set; }
		public bool SortDesc { get; set; }
	}

	public class ViewPermissionsQueryHandler : IRequestHandler<ViewPermissionsQuery, Result<PagedResponse<PermissionDTO>>> {
		private readonly IPermissionRepository _permissionRepository;

		public ViewPermissionsQueryHandler(IPermissionRepository permissionRepository) {
			_permissionRepository = permissionRepository;
		}

		public async Task<Result<PagedResponse<PermissionDTO>>> Handle(ViewPermissionsQuery request, CancellationToken cancellationToken) {
			var pagedPermissions = await _permissionRepository.SetNoTracking()
															  .ToPagedResponseAsync(request.Filter, request.PageNumber, request.PageSize,
																					request.SortBy, request.SortDesc);

			var pagedDTO = Map(pagedPermissions);

			return Result<PagedResponse<PermissionDTO>>.Success(pagedDTO);
		}
		private static PagedResponse<PermissionDTO> Map(PagedResponse<Permission> pagedPermissions) {

			var pagedDTO = new PagedResponse<PermissionDTO> {
				PageNumber = pagedPermissions.PageNumber,
				PageSize = pagedPermissions.PageSize,
				TotalRecords = pagedPermissions.TotalRecords,
				Items = pagedPermissions.Items.Select(permission => new PermissionDTO {
					Id = permission.Id,
					Name = permission.Name,
					Description = permission.Description,
				}).ToList()
			};

			return pagedDTO;
		}
	}
}

using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Administrator.DTOs;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Administrator.Queries {
	public class ViewRoleQuery : IRequest<Result<RoleDTO>> {
		public Guid RoleId { get; set; }
	}

	public class ViewRoleQueryHandler : IRequestHandler<ViewRoleQuery, Result<RoleDTO>> {

		private readonly IRoleRepository _roleRepository;

		public ViewRoleQueryHandler(IRoleRepository roleRepository) {
			_roleRepository = roleRepository;
		}

		public async Task<Result<RoleDTO>> Handle(ViewRoleQuery request, CancellationToken cancellationToken) {

			var role = await _roleRepository.GetRoleWithPermissionsNoTrackingAsync(request.RoleId, cancellationToken);

			if (role == null)
				return Result<RoleDTO>.Failure(Errors.RoleNotFound(request.RoleId));

			var roleDTO = new RoleDTO {
				Id = role.Id,
				Name = role.Name,
				Description = role.Description,
				Permissions = role.Permissions.Select(p => new PermissionDTO {
					Id = p.Id,
					Name = p.Name
				})
			};

			return Result<RoleDTO>.Success(roleDTO);
		}
	}

	public class ViewRoleQueryValidator : AbstractValidator<ViewRoleQuery> {
		public ViewRoleQueryValidator() {
			RuleFor(x => x.RoleId)
				.NotEmpty().WithMessage("Role ID is required.");
		}
	}
}

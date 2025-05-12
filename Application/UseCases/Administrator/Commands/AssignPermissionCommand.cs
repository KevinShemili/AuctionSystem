using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Commands {

	public class AssignPermissionCommand : IRequest<Result<bool>> {
		public Guid AdminId { get; set; }
		public Guid RoleId { get; set; }
		public List<Guid> PermissionIds { get; set; }
	}

	public class AssignPermissionCommandHandler : IRequestHandler<AssignPermissionCommand, Result<bool>> {

		private readonly IUnitOfWork _unitOfWork;
		private readonly IRoleRepository _roleRepository;
		private readonly IPermissionRepository _permissionRepository;
		private readonly ILogger<AssignPermissionCommandHandler> _logger;

		// Injecting the dependencies through the constructor.
		public AssignPermissionCommandHandler(ILogger<AssignPermissionCommandHandler> logger,
										IPermissionRepository permissionRepository,
										IRoleRepository roleRepository,
										IUnitOfWork unitOfWork) {
			_logger = logger;
			_permissionRepository = permissionRepository;
			_roleRepository = roleRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<Result<bool>> Handle(AssignPermissionCommand request, CancellationToken cancellationToken) {

			// Check if the role exists
			var role = await _roleRepository.GetRoleWithRolePermissionsAsync(request.RoleId, cancellationToken: cancellationToken);

			if (role is null) {
				_logger.LogWarning("Role with ID {RoleId} not found.", request.RoleId);
				return Result<bool>.Failure(Errors.RoleNotFound(request.RoleId));
			}

			// Check if the permissions exist
			var flag = await _permissionRepository.DoPermissionsExistAsync(request.PermissionIds, cancellationToken: cancellationToken);
			if (flag is false) {
				_logger.LogWarning("Permission list contains invalid IDs. Permissions: {Permissions}", request.PermissionIds);
				return Result<bool>.Failure(Errors.InvalidPermissions);
			}

			// Clear current permissions
			role.RolePermissions.Clear();

			// Assign new permissions to the role
			foreach (var permissionId in request.PermissionIds) {

				var rolePermission = new RolePermission {
					RoleId = request.RoleId,
					PermissionId = permissionId,
					AssignedBy = request.AdminId,
					DateCreated = DateTime.UtcNow
				};

				role.RolePermissions.Add(rolePermission);
			}

			// Persist the changes to the database
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken: cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class AssignPermissionCommandValidator : AbstractValidator<AssignPermissionCommand> {
		public AssignPermissionCommandValidator() {
			RuleFor(x => x.AdminId)
				.NotEmpty().WithMessage("Admin ID is required.");
			RuleFor(x => x.RoleId)
				.NotEmpty().WithMessage("Role ID is required.");
			RuleFor(x => x.PermissionIds)
				.NotEmpty().WithMessage("Permission IDs are required.");
		}
	}
}

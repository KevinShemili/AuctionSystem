using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Commands {
	public class AssignRoleCommand : IRequest<Result<bool>> {
		public Guid AdminId { get; set; }
		public Guid UserId { get; set; }
		public List<Guid> RoleIds { get; set; }
	}

	public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, Result<bool>> {

		private readonly IUnitOfWork _unitOfWork;
		private readonly IUserRepository _userRepository;
		private readonly IRoleRepository _roleRepository;
		private readonly ILogger<AssignRoleCommandHandler> _logger;

		public AssignRoleCommandHandler(IUnitOfWork unitOfWork,
								  IUserRepository userRepository,
								  ILogger<AssignRoleCommandHandler> logger,
								  IRoleRepository roleRepository) {
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
			_logger = logger;
			_roleRepository = roleRepository;
		}

		public async Task<Result<bool>> Handle(AssignRoleCommand request, CancellationToken cancellationToken) {

			var user = await _userRepository.GetUserWithUserRoles(request.UserId, cancellationToken: cancellationToken);

			if (user is null) {
				_logger.LogWarning("User with ID {UserId} not found.", request.UserId);
				return Result<bool>.Failure(Errors.UserNotFound(request.UserId));
			}

			if (user.IsAdministrator is false) {
				_logger.LogWarning("User with ID {UserId} is not an administrator.", request.UserId);
				return Result<bool>.Failure(Errors.OnlyAssignRolesToAdmin);
			}

			var flag = await _roleRepository.DoRolesExistAsync(request.RoleIds, cancellationToken: cancellationToken);
			if (flag is false) {
				_logger.LogWarning("Role list contains invalid IDs. Roles: {Roles}", request.RoleIds);
				return Result<bool>.Failure(Errors.InvalidRoles);
			}

			user.UserRoles.Clear();

			foreach (var roleId in request.RoleIds) {

				var userRole = new UserRole {
					UserId = request.UserId,
					RoleId = roleId,
					AssignedBy = request.AdminId,
					DateCreated = DateTime.UtcNow
				};

				user.UserRoles.Add(userRole);
			}

			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class AssignRoleCommandValidator : AbstractValidator<AssignRoleCommand> {
		public AssignRoleCommandValidator() {
			RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID cannot be empty.");
			RuleFor(x => x.RoleIds).NotEmpty().WithMessage("Role IDs cannot be empty.");
		}
	}
}

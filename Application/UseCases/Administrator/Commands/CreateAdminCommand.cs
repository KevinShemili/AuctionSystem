using Application.Common.EmailService;
using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.Tools.Passwords;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Administrator.Commands {
	public class CreateAdminCommand : IRequest<Result<bool>> {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public List<Guid> Roles { get; set; }
	}

	public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, Result<bool>> {

		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<CreateAdminCommandHandler> _logger;
		private readonly IRoleRepository _roleRepository;
		private readonly IEmailService _emailService;

		// Injecting the dependencies through the constructor.
		public CreateAdminCommandHandler(IUserRepository userRepository,
										IUnitOfWork unitOfWork,
										ILogger<CreateAdminCommandHandler> logger,
										IEmailService emailService,
										IRoleRepository roleRepository) {
			_userRepository = userRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
			_emailService = emailService;
			_roleRepository = roleRepository;
		}

		public async Task<Result<bool>> Handle(CreateAdminCommand request, CancellationToken cancellationToken) {

			// Check if the user with the provided email already exists
			var doesEmailExist = await _userRepository.DoesEmailExistAsync(request.Email, cancellationToken);

			if (doesEmailExist) {
				_logger.LogWarning("Email already exists. Email: {Email}", request.Email);
				return Result<bool>.Failure(Errors.EmailAlreadyExists);
			}

			// Check if administrator provided roles for this new creation.
			// A new admin must have at least one role.
			if (request.Roles.Any() is false) {
				_logger.LogWarning("No roles provided for the new admin. Request: {Request}", request);
				return Result<bool>.Failure(Errors.EmptyRoles);
			}

			// Check whether the provided role IDs are valid
			var flag = await _roleRepository.DoRolesExistAsync(request.Roles, cancellationToken);
			if (flag is false) {
				_logger.LogWarning("Role list contains invalid IDs. Roles: {Roles}", request.Roles);
				return Result<bool>.Failure(Errors.InvalidRoles);
			}

			// Generate a random password for the new admin
			var password = PasswordGenerator.Generate();

			// Hash it
			(var passwordHash, var passwordSalt) = Hasher.HashPasword(password);

			var user = new User {
				FirstName = request.FirstName,
				LastName = request.LastName,
				Email = request.Email,
				PasswordHash = passwordHash,
				PasswordSalt = passwordSalt,
				IsAdministrator = true,
				IsEmailVerified = true,
				UserRoles = new List<UserRole>()
			};

			foreach (var role in request.Roles) {
				user.UserRoles.Add(new UserRole {
					RoleId = role
				});
			}

			// Create & Persist the new admin
			_ = await _userRepository.CreateAsync(user, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			// Send the new admiin an email with his password
			await _emailService.SendAdminRegistrationEmailAsync(request.Email, password, cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class CreateAdminCommandValidator : AbstractValidator<CreateAdminCommand> {
		public CreateAdminCommandValidator() {
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required.");
			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required.");
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("A valid email address is required.");
		}
	}
}

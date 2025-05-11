using Application.Common.EmailService;
using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.TokenService;
using Application.Common.Tools.Passwords;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Authentication.Commands {
	public class RegisterCommand : IRequest<Result<bool>> {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
	}

	public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<bool>> {

		private readonly IUserRepository _userRepository;
		private readonly ITokenService _tokenService;
		private readonly IEmailService _emailService;
		private readonly IConfiguration _configuration;
		private readonly IVerificationTokenRepository _userTokenRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<RegisterCommandHandler> _logger;

		public RegisterCommandHandler(IUserRepository userRepository,
								ITokenService tokenService,
								IEmailService emailService,
								IConfiguration configuration,
								IVerificationTokenRepository userTokenRepository,
								IUnitOfWork unitOfWork,
								ILogger<RegisterCommandHandler> logger) {
			_userRepository = userRepository;
			_tokenService = tokenService;
			_emailService = emailService;
			_configuration = configuration;
			_userTokenRepository = userTokenRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Result<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken) {

			if (IsValidPassword(request.Password) is false) {
				_logger.LogWarning("Registration attempt failed: invalid password provided.");
				return Result<bool>.Failure(Errors.InvalidPasswordFormat);
			}

			// Check if email already exists
			var isExistingEmail = await _userRepository.DoesEmailExistAsync(request.Email, cancellationToken);

			if (isExistingEmail is true) {
				_logger.LogWarning("Registration attempt failed: Email {Email} already exists.", request.Email);
				return Result<bool>.Failure(Errors.EmailAlreadyExists);
			}

			// Generate email verification token
			var emailToken = _tokenService.GenerateEmailVerificationToken();

			// Map to domain user
			var user = new User {
				Email = request.Email,
				FirstName = request.FirstName,
				LastName = request.LastName,
				IsAdministrator = false,
			};

			// Hash password
			(user.PasswordHash, user.PasswordSalt) = Hasher.HashPasword(request.Password);

			// Add default initial wallet 10000$
			user.Wallet = new Wallet {
				Balance = 10000,
				DateCreated = DateTime.UtcNow,
			};

			// Create user
			_ = await _userRepository.CreateAsync(user, cancellationToken: cancellationToken);

			// Persist email verification token
			_ = await _userTokenRepository.CreateAsync(new VerificationToken {
				Token = emailToken,
				Expiry = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["VerificationTokenExpiries:ExpiryHours"])),
				UserId = user.Id,
				TokenTypeId = (int)VerificationTokenType.EmailVerificationToken
			}, cancellationToken: cancellationToken);

			// Send email
			await _emailService.SendConfirmationEmailAsync(emailToken, request.Email, cancellationToken);

			// Persist changes
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}

		private static bool IsValidPassword(string password) {
			if (string.IsNullOrEmpty(password)) return false;
			if (password.Length < 8 || password.Length > 50) return false;
			if (!password.Any(char.IsDigit)) return false;
			if (!password.Any(char.IsUpper)) return false;
			if (!password.Any(char.IsLower)) return false;
			return true;
		}
	}

	public class RegisterCommandValidator : AbstractValidator<RegisterCommand> {
		public RegisterCommandValidator() {
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.");
			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required.");
			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required.");
			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.");
		}
	}
}
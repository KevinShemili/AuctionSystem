using Application.Common.EmailService;
using Application.Common.ErrorMessages.AuthenticationUseCase;
using Application.Common.ResultPattern;
using Application.Common.TokenService;
using Application.Common.Tools.Hasher;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using AutoMapper;
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

		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;
		private readonly ITokenService _tokenService;
		private readonly IEmailService _emailService;
		private readonly IConfiguration _configuration;
		private readonly IUserTokenRepository _userTokenRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<RegisterCommandHandler> _logger;

		public RegisterCommandHandler(IUserRepository userRepository,
								IMapper mapper,
								ITokenService tokenService,
								IEmailService emailService,
								IConfiguration configuration,
								IUserTokenRepository userTokenRepository,
								IUnitOfWork unitOfWork,
								ILogger<RegisterCommandHandler> logger) {
			_userRepository = userRepository;
			_mapper = mapper;
			_tokenService = tokenService;
			_emailService = emailService;
			_configuration = configuration;
			_userTokenRepository = userTokenRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task<Result<bool>> Handle(RegisterCommand request, CancellationToken cancellationToken) {

			// Check if email already exists
			var isExistingEmail = await _userRepository.DoesEmailExistAsync(request.Email, cancellationToken);

			if (isExistingEmail is true) {
				_logger.LogWarning("Registration attempt failed: Email {Email} already exists.", request.Email);
				return Result<bool>.Failure(AuthenticationErrors.EmailAlreadyExists);
			}

			// Generate email verification token
			var emailToken = _tokenService.GenerateEmailVerificationToken();

			// Map to domain user
			var user = _mapper.Map<User>(request);

			// Hash password
			(user.PasswordHash, user.PasswordSalt) = Hasher.HashPasword(request.Password);

			// Add default initial wallet 10000$
			user.Wallet = new Wallet {
				Balance = 10000
			};

			// Create user
			_ = await _userRepository.CreateAsync(user, cancellationToken: cancellationToken);

			// Persist email verification token
			_ = await _userTokenRepository.CreateAsync(new UserToken {
				Token = emailToken,
				Expiry = DateTime.UtcNow.AddHours(
						Convert.ToDouble(_configuration["VerificationTokenExpiries:ExpiryHours"])),
				UserId = user.Id,
				TokenTypeId = (int)TokenTypeEnum.EmailVerificationToken
			}, cancellationToken: cancellationToken);

			// Send email
			await _emailService.SendConfirmationEmailAsync(emailToken, request.Email, cancellationToken);

			// Persist changes
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class RegisterCommandValidator : AbstractValidator<RegisterCommand> {
		public RegisterCommandValidator() {

			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.")
				.EmailAddress().WithMessage("Email format is invalid.");

			RuleFor(x => x.FirstName)
				.NotEmpty().WithMessage("First name is required.")
				.MinimumLength(2).WithMessage("First name must be at least 2 characters.")
				.MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

			RuleFor(x => x.LastName)
				.NotEmpty().WithMessage("Last name is required.")
				.MinimumLength(2).WithMessage("Last name must be at least 2 characters.")
				.MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.")
				.MinimumLength(8).WithMessage("Password must be at least 8 characters.")
				.MaximumLength(50).WithMessage("Password must not exceed 50 characters.")
				.Must(x => x.Any(char.IsDigit)).WithMessage("Password must contain at least one digit.")
				.Must(x => x.Any(char.IsUpper)).WithMessage("Password must contain at least one uppercase letter.")
				.Must(x => x.Any(char.IsLower)).WithMessage("Password must contain at least one lowercase letter.");
		}
	}
}
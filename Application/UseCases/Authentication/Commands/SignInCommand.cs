using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.TokenService;
using Application.Common.Tools.Passwords;
using Application.Common.Tools.Transcode;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Application.UseCases.Authentication.DTOs;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Authentication.Commands {
	public class SignInCommand : IRequest<Result<SignInDTO>> {
		public string Email { get; set; }
		public string Password { get; set; }
	}

	public class SignInCommandHandler : IRequestHandler<SignInCommand, Result<SignInDTO>> {

		private readonly ITokenService _tokenService;
		private readonly IConfiguration _configuration;
		private readonly IUserRepository _userRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<SignInCommandHandler> _logger;
		private readonly IAuthenticationTokenRepository _authenticationTokenRepository;

		// Injecting the dependencies through the constructor.
		public SignInCommandHandler(ITokenService tokenService,
							  IConfiguration configuration,
							  IUnitOfWork unitOfWork,
							  ILogger<SignInCommandHandler> logger,
							  IUserRepository userRepository,
							  IAuthenticationTokenRepository authenticationTokenRepository) {
			_tokenService = tokenService;
			_configuration = configuration;
			_unitOfWork = unitOfWork;
			_logger = logger;
			_userRepository = userRepository;
			_authenticationTokenRepository = authenticationTokenRepository;
		}

		public async Task<Result<SignInDTO>> Handle(SignInCommand request, CancellationToken cancellationToken) {

			// Get the user by email
			var user = await _userRepository.GetUserWithAuthenticationTokensAsync(request.Email, cancellationToken: cancellationToken);

			// Check if the user exists
			if (user is null) {
				_logger.LogWarning("SignIn attempt failed: Email {Email} does not exist.", request.Email);
				return Result<SignInDTO>.Failure(Errors.UserNotFound(request.Email));
			}

			// Check if the user has verified their email
			if (user.IsEmailVerified is false) {
				_logger.LogWarning("SignIn attempt failed: Email {Email} is not verified.", request.Email);
				return Result<SignInDTO>.Failure(Errors.AccountNotVerified);
			}

			// Check if the user is blocked
			if (user.IsBlocked is true) {
				_logger.LogWarning("SignIn attempt failed: User {FirstName} {LastName} is blocked.", user.FirstName, user.LastName);
				return Result<SignInDTO>.Failure(Errors.LockedOut);
			}

			// Verify the password
			var isPasswordCorrect = Hasher.VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt);

			// The user may provide a wrong password only a few times
			if (isPasswordCorrect is false) {
				var maxTries = int.Parse(_configuration["FailedLogin:MaxTries"]);

				// Check if the user has reached the max tries -> block
				if (user.FailedLoginTries >= maxTries) {

					user.IsBlocked = true;
					user.BlockReason = "Max login attempts reached.";

					_ = await _unitOfWork.SaveChangesAsync(cancellationToken);
					_logger.LogWarning("User {FirstName} {LastName} reached max tries: Blocked.", user.FirstName, user.LastName);
					return Result<SignInDTO>.Failure(Errors.BlockReason(user.BlockReason));
				}

				user.FailedLoginTries += 1;

				_ = await _userRepository.UpdateAsync(user, true, cancellationToken);

				return Result<SignInDTO>.Failure(Errors.SignInFailure);
			}

			// Reset the failed login tries if the password is correct
			user.FailedLoginTries = 0;

			// Provide JWT & Refresh token
			var accessToken = await _tokenService.GenerateAccessTokenAsync(request.Email, cancellationToken);
			(var refreshToken, var refreshExpiry) = _tokenService.GenerateRefreshToken();

			var lastestRefreshToken = user.AuthenticationTokens.OrderByDescending(x => x.DateCreated)
															   .FirstOrDefault();

			// Delete old auth tokens if any
			if (lastestRefreshToken is not null)
				_ = await _authenticationTokenRepository.DeleteAsync(lastestRefreshToken, cancellationToken: cancellationToken);

			user.AuthenticationTokens.Add(new AuthenticationToken {
				RefreshToken = refreshToken,
				Expiry = refreshExpiry,
				AccessToken = accessToken,
				DateCreated = DateTime.UtcNow,
			});

			// Persist the changes
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<SignInDTO>.Success(new SignInDTO {
				AccessToken = accessToken,
				RefreshToken = Transcode.EncodeURL(refreshToken)
			});
		}
	}

	public class SignInCommandValidator : AbstractValidator<SignInCommand> {
		public SignInCommandValidator() {
			RuleFor(x => x.Email)
				.NotEmpty().WithMessage("Email is required.");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Password is required.");
		}
	}
}
using Application.Common.EmailService;
using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.TokenService;
using Application.Common.Tools.Transcode;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Domain.Entities;
using Domain.Enumerations;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Authentication.Commands {
	public class ConfirmEmailCommand : IRequest<Result<bool>> {
		public string Email { get; set; }
		public string Token { get; set; }
	}

	public class ConfirmEmailCommandHandler : IRequestHandler<ConfirmEmailCommand, Result<bool>> {

		private readonly ITokenService _tokenService;
		private readonly IEmailService _emailService;
		private readonly IConfiguration _configuration;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<ConfirmEmailCommandHandler> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IVerificationTokenRepository _userTokenRepository;

		public ConfirmEmailCommandHandler(IConfiguration configuration,
									IEmailService emailService,
									ITokenService tokenService,
									ILogger<ConfirmEmailCommandHandler> logger,
									IUnitOfWork unitOfWork,
									IUserRepository userRepository,
									IVerificationTokenRepository userTokenRepository) {
			_configuration = configuration;
			_emailService = emailService;
			_tokenService = tokenService;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_userRepository = userRepository;
			_userTokenRepository = userTokenRepository;
		}

		public async Task<Result<bool>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken) {

			// Get the user verification token by email
			var user = await _userRepository.GetUserWithTokensAsync(request.Email, cancellationToken);

			// Check if the user exists
			if (user is null) {
				_logger.LogWarning("Email Verification attempt failed. Email: {Email} Token: {Token}", request.Email, request.Token);
				return Result<bool>.Failure(Errors.InvalidToken);
			}

			// Check if the user is already verified
			if (user.IsEmailVerified is true) {
				_logger.LogWarning("Email Verification attempt failed. Email: {Email} Token: {Token}", request.Email, request.Token);
				return Result<bool>.Failure(Errors.AccountAlreadyVerified);
			}

			// Decode the token
			var decodedToken = Transcode.DecodeURL(request.Token);

			// Check if such token has been issued for the user
			var token = user.VerificationTokens.FirstOrDefault(x => x.Token == decodedToken &&
																	x.TokenTypeId == (int)VerificationTokenType.EmailVerificationToken);

			// If not issued, return failure
			if (token is null) {
				_logger.LogWarning("Email Verification attempt failed. User: {Email}", user.Email);
				return Result<bool>.Failure(Errors.InvalidToken);
			}

			// Check if the token is expired
			if (DateTime.UtcNow > token.Expiry) {

				// If expired, delete the token and send a new one
				var emailToken = _tokenService.GenerateEmailVerificationToken();

				user.VerificationTokens.Add(new VerificationToken {
					Token = emailToken,
					Expiry = DateTime.UtcNow.AddHours(
						Convert.ToDouble(_configuration["VerificationTokenExpiries:ExpiryHours"])),
					TokenTypeId = (int)VerificationTokenType.EmailVerificationToken,
				});

				await _emailService.SendConfirmationEmailAsync(emailToken, request.Email, cancellationToken);

				_ = await _userTokenRepository.DeleteAsync(token, cancellationToken: cancellationToken);
				_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

				_logger.LogWarning("Email Verification attempt failed, expired token. Email: {Email}", user.Email);
				return Result<bool>.Failure(Errors.ExpiredEmailToken);
			}

			// Verify the user's email
			user.IsEmailVerified = true;

			_ = await _userTokenRepository.DeleteAsync(token, cancellationToken: cancellationToken);
			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand> {
		public ConfirmEmailCommandValidator() {
			RuleFor(x => x.Email)
				.NotEmpty();

			RuleFor(x => x.Token)
				.NotEmpty();
		}
	}
}

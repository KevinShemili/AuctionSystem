using Application.Common.ErrorMessages.AuthenticationUseCase;
using Application.Common.ResultPattern;
using Application.Common.TokenService;
using Application.Common.Tools.Transcode;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Application.UseCases.Authentication.Results;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.UseCases.Authentication.Commands {
	public class RefreshTokenCommand : IRequest<Result<RefreshTokenCommandResult>> {
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}

	public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenCommandResult>> {

		private readonly ITokenService _tokenService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<RefreshTokenCommandHandler> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IAuthenticationTokenRepository _authenticationTokenRepository;

		public RefreshTokenCommandHandler(ILogger<RefreshTokenCommandHandler> logger,
									IUnitOfWork unitOfWork,
									ITokenService tokenService,
									IUserRepository userRepository,
									IAuthenticationTokenRepository authenticationTokenRepository) {
			_logger = logger;
			_unitOfWork = unitOfWork;
			_tokenService = tokenService;
			_userRepository = userRepository;
			_authenticationTokenRepository = authenticationTokenRepository;
		}

		public async Task<Result<RefreshTokenCommandResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) {

			ClaimsPrincipal claims;

			try {
				claims = _tokenService.GetClaims(request.AccessToken);
			}
			catch (Exception) {
				return Result<RefreshTokenCommandResult>.Failure(AuthenticationErrors.InvalidToken);
			}

			var email = claims.FindFirst(ClaimTypes.Email)!.Value;

			var user = await _userRepository.GetUserWithAuthenticationTokensNoTrackingAsync(email, cancellationToken);

			// Failsafe, but should never happen
			if (user is null)
				return Result<RefreshTokenCommandResult>.Failure(AuthenticationErrors.ServerError);

			var decodedRefreshToken = Transcode.DecodeURL(request.RefreshToken);

			var currentRefreshToken = user.AuthenticationTokens.FirstOrDefault(x => x.RefreshToken == decodedRefreshToken);

			if (currentRefreshToken is null)
				return Result<RefreshTokenCommandResult>.Failure(AuthenticationErrors.Unauthorized);

			if (currentRefreshToken.AccessToken != request.AccessToken)
				return Result<RefreshTokenCommandResult>.Failure(AuthenticationErrors.Unauthorized);

			if (currentRefreshToken.Expiry >= DateTime.UtcNow)
				return Result<RefreshTokenCommandResult>.Failure(AuthenticationErrors.Unauthorized);

			var newAccessToken = _tokenService.GenerateAccessToken(claims.Claims);
			(var newRefreshToken, var expiry) = _tokenService.GenerateRefreshToken();

			_ = await _authenticationTokenRepository.DeleteAsync(currentRefreshToken, cancellationToken: cancellationToken);
			_ = await _authenticationTokenRepository.CreateAsync(new AuthenticationToken {
				RefreshToken = newRefreshToken,
				AccessToken = newAccessToken,
				Expiry = expiry,
				UserId = user.Id,
			}, cancellationToken: cancellationToken);

			_ = await _unitOfWork.SaveChangesAsync(cancellationToken);

			return Result<RefreshTokenCommandResult>.Success(new RefreshTokenCommandResult {
				AccessToken = newAccessToken,
				RefreshToken = Transcode.EncodeURL(newRefreshToken)
			});
		}
	}

	public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand> {
		public RefreshTokenCommandValidator() {
			RuleFor(x => x.AccessToken)
				.NotEmpty();

			RuleFor(x => x.RefreshToken)
				.NotEmpty();
		}
	}
}

using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Common.TokenService;
using Application.Common.Tools.Transcode;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Application.UseCases.Authentication.DTOs;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.UseCases.Authentication.Commands {
	public class RefreshTokenCommand : IRequest<Result<RefreshTokenDTO>> {
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}

	public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenDTO>> {

		private readonly ITokenService _tokenService;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILogger<RefreshTokenCommandHandler> _logger;
		private readonly IUserRepository _userRepository;
		private readonly IAuthenticationTokenRepository _authenticationTokenRepository;

		// Injecting the dependencies through the constructor.
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

		public async Task<Result<RefreshTokenDTO>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken) {

			ClaimsPrincipal claims;

			// Get the claims from the JWT 
			try {
				claims = _tokenService.GetClaims(request.AccessToken);
			}
			catch (Exception) {
				_logger.LogWarning("Refresh Token Command failed. RefreshToken: {RefreshToken} AccessToken: {AccessToken}", request.RefreshToken, request.AccessToken);
				return Result<RefreshTokenDTO>.Failure(Errors.InvalidToken);
			}

			// Get the user from the JWT
			var email = claims.FindFirst(ClaimTypes.Email)!.Value;

			var user = await _userRepository.GetUserWithAuthenticationTokensAsync(email, cancellationToken);

			// Failsafe, but should never happen
			if (user is null) {
				_logger.LogCritical("This command has been hit, with a valid a valid JWT & Refresh, although no such user exists in the system" +
					"User: {User}. AccessToken: {AccessToken}, RefreshToken: {RefreshToken}"
					, email, request.AccessToken, request.RefreshToken);
				return Result<RefreshTokenDTO>.Failure(Errors.ServerError);
			}

			// Decode the refresh token
			var decodedRefreshToken = Transcode.DecodeURL(request.RefreshToken);

			// Check if the refresh token is valid
			var currentRefreshToken = user.AuthenticationTokens.FirstOrDefault(x => x.RefreshToken == decodedRefreshToken);

			if (currentRefreshToken is null) {
				_logger.LogWarning("Refresh Token Command failed. No refresh available, re-login: User: {User}", user.Email);
				return Result<RefreshTokenDTO>.Failure(Errors.Unauthorized);
			}

			if (currentRefreshToken.AccessToken != request.AccessToken) {
				_logger.LogWarning("Unauthorized request. User: {User} AccessToken: {AccessToken} RefreshToken: {RefreshToken}", user.Email, request.AccessToken, request.RefreshToken);
				return Result<RefreshTokenDTO>.Failure(Errors.Unauthorized);
			}

			// Check if the refresh token is expired -> Perform normal login
			if (currentRefreshToken.Expiry <= DateTime.UtcNow) {
				_logger.LogWarning("Refresh Token Command failed. Expired refresh. User: {User}", user.Email);
				return Result<RefreshTokenDTO>.Failure(Errors.Unauthorized);
			}

			// Generate new JWT & refresh token
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

			return Result<RefreshTokenDTO>.Success(new RefreshTokenDTO {
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

using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Authentication.Commands {
	public class RevokeRefreshTokenCommand : IRequest<Result<bool>> {
		public Guid UserId { get; set; }
	}

	public class RevokeRefreshTokenCommandHandler : IRequestHandler<RevokeRefreshTokenCommand, Result<bool>> {

		private readonly IAuthenticationTokenRepository _authenticationTokenRepository;
		private readonly ILogger<RevokeRefreshTokenCommandHandler> _logger;

		public RevokeRefreshTokenCommandHandler(ILogger<RevokeRefreshTokenCommandHandler> logger,
										  IAuthenticationTokenRepository authenticationTokenRepository) {
			_logger = logger;
			_authenticationTokenRepository = authenticationTokenRepository;
		}

		public async Task<Result<bool>> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken) {

			var refreshToken = await _authenticationTokenRepository.GetByUserID(request.UserId, cancellationToken);

			if (refreshToken is null) {
				_logger.LogWarning("Revoke Refresh Command failed. No refresh available for user. User: {UserId}", request.UserId);
				return Result<bool>.Failure(Errors.UserNotFound(request.UserId));
			}

			_ = await _authenticationTokenRepository.DeleteAsync(refreshToken, true, cancellationToken);

			return Result<bool>.Success(true);
		}
	}

	public class RevokeRefreshCommandValidator : AbstractValidator<RevokeRefreshTokenCommand> {
		public RevokeRefreshCommandValidator() {
			RuleFor(x => x.UserId)
				.NotEmpty();
		}
	}
}

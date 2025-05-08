using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Profile.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.UseCases.Profile.Queries {
	public class GetProfileQuery : IRequest<Result<ProfileDTO>> {
		public Guid UserId { get; set; }
	}

	public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, Result<ProfileDTO>> {

		private readonly IUserRepository _userRepository;
		private readonly ILogger<GetProfileQueryHandler> _logger;

		public GetProfileQueryHandler(ILogger<GetProfileQueryHandler> logger, IUserRepository userRepository) {
			_logger = logger;
			_userRepository = userRepository;
		}

		public async Task<Result<ProfileDTO>> Handle(GetProfileQuery request, CancellationToken cancellationToken) {

			var user = await _userRepository.GetUserWithWalletNoTrackingAsync(request.UserId, cancellationToken);

			if (user is null) {
				_logger.LogCritical("User bypassed authorization. {UserId} not found", request.UserId);
				return Result<ProfileDTO>.Failure(Errors.Unauthorized);
			}

			return Result<ProfileDTO>.Success(MapResponse(user));
		}

		private static ProfileDTO MapResponse(User user) {

			var profile = new ProfileDTO {
				Id = user.Id,
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email,
				WalletId = user.Wallet.Id,
				Balance = user.Wallet.Balance,
				FrozenBalance = user.Wallet.FrozenBalance
			};

			return profile;
		}
	}
}

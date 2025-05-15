using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Profile.DTOs;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Profile.Queries {
	public class ViewMyWalletQuery : IRequest<Result<WalletDTO>> {
		public Guid UserId { get; set; }
	}

	public class ViewMyWalletQueryHandler : IRequestHandler<ViewMyWalletQuery, Result<WalletDTO>> {

		private readonly IUserRepository _userRepository;

		public ViewMyWalletQueryHandler(IUserRepository userRepository) {
			_userRepository = userRepository;
		}

		public async Task<Result<WalletDTO>> Handle(ViewMyWalletQuery request, CancellationToken cancellationToken) {

			// Get the user with:
			// 1. Wallet -> Transactions
			var user = await _userRepository.GetUserWithWalletAndTransactionsNoTrackingAsync(request.UserId, cancellationToken);

			// Check if the user exists
			if (user is null) {
				return Result<WalletDTO>.Failure(Errors.UserNotFound(request.UserId));
			}

			// Check if the user is administrator
			if (user.IsAdministrator is true) {
				return Result<WalletDTO>.Failure(Errors.NotAccessibleByAdmins);
			}

			var wallet = user.Wallet;

			// Map to DTO
			var walletDto = new WalletDTO {
				Id = wallet.Id,
				Balance = wallet.Balance,
				FrozenBalance = wallet.FrozenBalance,
				Transactions = wallet.Transactions.Select(x => new TransactionsDTO {
					Id = x.Id,
					Amount = x.Amount,
					TransactionType = x.TransactionType
				}).ToList()
			};

			return Result<WalletDTO>.Success(walletDto);
		}
	}

	public class ViewMyWalletQueryValidator : AbstractValidator<ViewMyWalletQuery> {
		public ViewMyWalletQueryValidator() {
			RuleFor(x => x.UserId)
				.NotEmpty().WithMessage("User ID cannot be empty.");
		}
	}
}

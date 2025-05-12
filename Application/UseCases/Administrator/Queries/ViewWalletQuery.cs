using Application.Common.ErrorMessages;
using Application.Common.ResultPattern;
using Application.Contracts.Repositories;
using Application.UseCases.Profile.DTOs;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Administrator.Queries {
	public class ViewWalletQuery : IRequest<Result<WalletDTO>> {
		public Guid WalletId { get; set; }
	}

	public class ViewWalletQueryHandler : IRequestHandler<ViewWalletQuery, Result<WalletDTO>> {

		private readonly IWalletRepository _walletRepository;

		// Injecting the dependencies through the constructor.
		public ViewWalletQueryHandler(IWalletRepository walletRepository) {
			_walletRepository = walletRepository;
		}

		public async Task<Result<WalletDTO>> Handle(ViewWalletQuery request, CancellationToken cancellationToken) {

			// Get wallet with:
			// 1. Transactions
			var wallet = await _walletRepository.GetWalletWithTransactionsNoTrackingAsync(request.WalletId, cancellationToken);

			// Check if the wallet exists
			if (wallet is null) {
				return Result<WalletDTO>.Failure(Errors.WalletNotFound(request.WalletId));
			}

			// Map result to DTO
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

	public class ViewWalletQueryValidator : AbstractValidator<ViewWalletQuery> {
		public ViewWalletQueryValidator() {
			RuleFor(x => x.WalletId)
				.NotEmpty().WithMessage("Wallet ID cannot be empty.");
		}
	}
}

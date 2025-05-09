using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IWalletRepository : IRepository<Wallet> {
		Task<Wallet> GetWalletWithTransactionsNoTrackingAsync(Guid walletId, CancellationToken cancellationToken = default);
	}
}

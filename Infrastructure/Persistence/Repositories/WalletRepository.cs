using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class WalletRepository : BaseRepository<Wallet>, IWalletRepository {
		public WalletRepository(DatabaseContext context) : base(context) {
		}

		public async Task<Wallet> GetWalletWithTransactionsNoTrackingAsync(Guid walletId, CancellationToken cancellationToken = default) {

			if (walletId == Guid.Empty)
				throw new ArgumentException("Wallet ID cannot be empty.", nameof(walletId));

			var wallet = await SetNoTracking().Include(w => w.Transactions)
											  .FirstOrDefaultAsync(w => w.Id == walletId, cancellationToken);

			return wallet;
		}
	}
}

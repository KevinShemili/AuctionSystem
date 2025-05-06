using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class WalletTransactionRepository : BaseRepository<WalletTransaction>, IWalletTransactionRepository {
		public WalletTransactionRepository(DatabaseContext context) : base(context) {
		}
	}
}

using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class WalletRepository : BaseRepository<Wallet>, IWalletRepository {
		public WalletRepository(DatabaseContext context) : base(context) {
		}
	}
}

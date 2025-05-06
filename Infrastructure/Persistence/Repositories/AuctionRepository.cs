using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class AuctionRepository : BaseRepository<Auction>, IAuctionRepository {
		public AuctionRepository(DatabaseContext context) : base(context) {
		}
	}
}

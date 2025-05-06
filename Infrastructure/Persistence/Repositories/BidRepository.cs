using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class BidRepository : BaseRepository<Bid>, IBidRepository {
		public BidRepository(DatabaseContext context) : base(context) {
		}
	}
}

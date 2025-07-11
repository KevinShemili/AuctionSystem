using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class BidRepository : BaseRepository<Bid>, IBidRepository {
		public BidRepository(DatabaseContext context) : base(context) {
		}

		public IQueryable<Bid> GetAllByBidderNoTracking(Guid userId) {

			if (userId == Guid.Empty)
				throw new ArgumentNullException(nameof(userId));

			var bids = SetNoTracking().Include(x => x.Auction)
									  .Where(x => x.BidderId == userId);

			return bids;
		}
	}
}

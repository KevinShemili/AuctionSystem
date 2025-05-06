using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class AuctionRepository : BaseRepository<Auction>, IAuctionRepository {
		public AuctionRepository(DatabaseContext context) : base(context) {
		}

		public async Task<Auction> GetAuctionWithBidsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default) {

			if (id == Guid.Empty)
				throw new ArgumentNullException(nameof(id));

			var auction = await SetNoTracking().Include(x => x.Bids)
											   .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			return auction;
		}
	}
}

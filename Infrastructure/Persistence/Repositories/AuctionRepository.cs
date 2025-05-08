using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class AuctionRepository : BaseRepository<Auction>, IAuctionRepository {
		public AuctionRepository(DatabaseContext context) : base(context) {
		}

		public async Task<Auction> GetAuctionWithBidsAndSellerNoTrackingAsync(Guid id, CancellationToken cancellationToken = default) {

			if (id == Guid.Empty)
				throw new ArgumentNullException(nameof(id));

			var auction = await SetNoTracking().Include(x => x.Bids)
											   .Include(x => x.Seller)
											   .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			return auction;
		}

		public async Task<IEnumerable<Auction>> GetExpiredAuctionsAsync(DateTime endTime, CancellationToken cancellationToken = default) {

			var auctions = await SetTracking().Include(x => x.Bids)
												  .ThenInclude(x => x.Bidder)
												  .ThenInclude(x => x.Wallet)
											  .Include(x => x.Seller)
												  .ThenInclude(x => x.Wallet)
											  .Where(x => x.Status == (int)AuctionStatusEnum.Active &&
														  x.EndTime <= endTime)
											  .ToListAsync(cancellationToken);

			return auctions;
		}
	}
}

using Application.Contracts.Repositories;
using Domain.Entities;
using Domain.Enumerations;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class AuctionRepository : BaseRepository<Auction>, IAuctionRepository {
		public AuctionRepository(DatabaseContext context) : base(context) {
		}

		public async Task<IEnumerable<Auction>> GetAuctionsCreatedByUserNoTrackingAsync(Guid userId, CancellationToken cancellationToken = default) {

			if (userId == Guid.Empty)
				throw new ArgumentNullException(nameof(userId));

			var auctions = await SetNoTracking().Include(x => x.Images)
												.Where(x => x.SellerId == userId)
												.ToListAsync(cancellationToken);

			return auctions;
		}

		public async Task<IEnumerable<Auction>> GetAuctionsPartecipatedByUserNoTrackingAsync(Guid userId, CancellationToken cancellationToken = default) {

			if (userId == Guid.Empty)
				throw new ArgumentNullException(nameof(userId));

			var auctions = await SetNoTracking().Include(x => x.Images)
												.Include(x => x.Bids)
												.Where(x => x.Bids.Any(x => x.BidderId == userId))
												.ToListAsync(cancellationToken);

			return auctions;
		}

		public async Task<Auction> GetAuctionWithBidsSellerWalletTransactionsAsync(Guid id, CancellationToken cancellationToken = default) {

			if (id == Guid.Empty)
				throw new ArgumentNullException(nameof(id));

			var auction = await SetTracking().Include(x => x.Bids)
													.ThenInclude(x => x.Bidder)
													.ThenInclude(x => x.Wallet)
													.ThenInclude(x => x.Transactions)
											   .Include(x => x.Seller)
											   .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			return auction;
		}

		public async Task<Auction> GetAuctionWithBidsSellerNoTrackingAsync(Guid id, CancellationToken cancellationToken = default) {

			if (id == Guid.Empty)
				throw new ArgumentNullException(nameof(id));

			var auction = await SetNoTracking().Include(x => x.Bids)
													.ThenInclude(x => x.Bidder)
											   .Include(x => x.Seller)
											   .Include(x => x.Images)
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

		public IQueryable<Auction> GetAllNoTracking(bool activeOnly) {

			var auctions = activeOnly ? SetNoTracking().Include(x => x.Images)
													   .Include(x => x.Bids)
													   .Where(x => x.Status == (int)AuctionStatusEnum.Active) :

										SetNoTracking().Include(x => x.Images)
													   .Include(x => x.Bids)
													   .Where(x => x.Status == (int)AuctionStatusEnum.Active ||
																   x.Status == (int)AuctionStatusEnum.Ended);

			return auctions;
		}

		public async Task<Auction> GetAuctionWithBidsAsync(Guid id, CancellationToken cancellationToken = default) {

			if (id == Guid.Empty)
				throw new ArgumentNullException(nameof(id));

			var auction = await SetTracking().Include(x => x.Bids)
											 .Include(x => x.Images)
											 .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			return auction;
		}
	}
}

using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IAuctionRepository : IRepository<Auction> {
		Task<Auction> GetAuctionWithBidsSellerWalletTransactionsNoTrackingAsync(Guid id, CancellationToken cancellationToken = default);
		Task<IEnumerable<Auction>> GetExpiredAuctionsAsync(DateTime endTime, CancellationToken cancellationToken = default);
		Task<IEnumerable<Auction>> GetAuctionsCreatedByUserNoTrackingAsync(Guid userId, CancellationToken cancellationToken = default);
		Task<IEnumerable<Auction>> GetAuctionsPartecipatedByUserNoTrackingAsync(Guid userId, CancellationToken cancellationToken = default);
		Task<Auction> GetAuctionWithBidsSellerNoTrackingAsync(Guid id, CancellationToken cancellationToken = default);
	}
}

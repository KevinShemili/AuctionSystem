using Domain.Common;

namespace Domain.Entities {
	public class Bid : AbstractEntity {

		// Fields
		public decimal Amount { get; set; }
		public bool IsWinningBid { get; set; }

		// Foreign Keys
		public Guid AuctionId { get; set; }
		public Guid BidderId { get; set; }

		// Relationships
		public virtual Auction Auction { get; set; }
		public virtual User Bidder { get; set; }
		public virtual ICollection<WalletTransaction> Transactions { get; set; }
	}
}

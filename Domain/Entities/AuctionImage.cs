using Domain.Common;

namespace Domain.Entities {
	public class AuctionImage : AbstractEntity {
		// Fields

		// Foreign Keys
		public Guid AuctionId { get; set; }

		// Relationships
		public virtual Auction Auction { get; set; }
	}
}

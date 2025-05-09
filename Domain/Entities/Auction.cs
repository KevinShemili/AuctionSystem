using Domain.Common;

namespace Domain.Entities {
	public class Auction : AbstractEntity {
		// Fields
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }
		public Guid? ForceClosedBy { get; set; }
		public string ForceClosedReason { get; set; }

		// Foreign Keys
		public Guid SellerId { get; set; }

		// Relationships
		public virtual User Seller { get; set; }
		public virtual ICollection<Bid> Bids { get; set; }
		public virtual ICollection<AuctionImage> Images { get; set; }
	}
}

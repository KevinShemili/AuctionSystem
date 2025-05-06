using Domain.Common;

namespace Domain.Entities {
	public class WalletTransaction : AbstractEntity {

		// Fields
		public decimal Amount { get; set; }
		public int TransactionType { get; set; }

		// Foreign Keys
		public Guid WalletId { get; set; }
		public Guid BidId { get; set; }

		// Relationships
		public virtual Wallet Wallet { get; set; }
		public virtual Bid Bid { get; set; }
	}
}

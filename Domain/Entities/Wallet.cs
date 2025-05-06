using Domain.Common;

namespace Domain.Entities {
	public class Wallet : AbstractEntity {

		// Fields
		public decimal Balance { get; set; }
		public decimal FrozenBalance { get; set; }

		// Foreign Keys
		public Guid UserId { get; set; }

		// Relationships
		public virtual User User { get; set; }
		public virtual ICollection<WalletTransaction> Transactions { get; set; }
	}
}

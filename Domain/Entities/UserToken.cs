using Domain.Common;

namespace Domain.Entities {
	public class UserToken : AbstractEntity {

		// Fields
		public string Token { get; set; }
		public DateTime Expiry { get; set; }
		public int TokenTypeId { get; set; }

		// Foreign Keys
		public Guid UserId { get; set; }

		// Relationships
		public virtual User User { get; set; }
	}
}

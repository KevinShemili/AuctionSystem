using Domain.Common;

namespace Domain.Entities {
	public class AuthenticationToken : AbstractEntity {

		// Fields
		public string RefreshToken { get; set; }
		public DateTime Expiry { get; set; }
		public string AccessToken { get; set; }

		// Foreign Keys
		public Guid UserId { get; set; }

		// Relationships
		public virtual User User { get; set; }
	}
}

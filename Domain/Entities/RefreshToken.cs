using Domain.Common;

namespace Domain.Entities {
	public class RefreshToken : EntityBase {
		public required string Token { get; set; }
		public DateTime Expiry { get; set; }
		public required string AccessToken { get; set; }
		public int UserId { get; set; }

		public virtual User User { get; set; }
	}
}

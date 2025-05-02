using Domain.Common;

namespace Domain.Entities {
	public class UserToken : EntityBase {
		public string Token { get; set; }
		public DateTime Expiry { get; set; }
		public int TokenTypeId { get; set; }
		public int UserId { get; set; }

		public virtual User User { get; set; }
	}
}

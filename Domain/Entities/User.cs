using Domain.Common;

namespace Domain.Entities {
	public class User : AbstractEntity {

		// Fields
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public bool IsEmailVerified { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public int FailedLoginTries { get; set; }
		public bool IsBlocked { get; set; }
		public string BlockReason { get; set; }
		public bool IsAdministrator { get; set; }

		// Relationships
		public virtual ICollection<Role> Roles { get; set; }
		public virtual ICollection<UserRole> UserRoles { get; set; }
		public virtual ICollection<AuthenticationToken> AuthenticationTokens { get; set; }
		public virtual ICollection<VerificationToken> VerificationTokens { get; set; }
		public virtual Wallet Wallet { get; set; }
		public virtual ICollection<Auction> Auctions { get; set; }
		public virtual ICollection<Bid> Bids { get; set; }
	}
}

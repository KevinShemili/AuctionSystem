namespace Domain.Entities {
	public class User {
		public string UserName { get; set; }
		public string Email { get; set; }
		public bool IsEmailVerified { get; set; }
		public string PasswordHash { get; set; }
		public string PasswordSalt { get; set; }
		public int FailedLoginTries { get; set; }
		public bool IsBlocked { get; set; }

		public virtual ICollection<Role> Roles { get; set; }
		public virtual ICollection<UserRole> UserRoles { get; set; }
		public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
		public virtual ICollection<UserToken> UserTokens { get; set; }
	}
}

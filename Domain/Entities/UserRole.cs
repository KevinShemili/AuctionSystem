namespace Domain.Entities {
	public class UserRole {
		public int UserId { get; set; }
		public int RoleId { get; set; }
		public Guid? AssignedBy { get; set; }
		public string AssignedByName { get; set; }

		public virtual User User { get; set; }
		public virtual Role Role { get; set; }
	}
}

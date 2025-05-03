using Domain.Common;

namespace Domain.Entities {
	public class UserRole : EntityBase {

		// Fields
		public Guid? AssignedBy { get; set; }
		public string AssignedByName { get; set; }

		// Foreign Keys
		public Guid UserId { get; set; }
		public Guid RoleId { get; set; }

		// Relationships
		public virtual User User { get; set; }
		public virtual Role Role { get; set; }
	}
}

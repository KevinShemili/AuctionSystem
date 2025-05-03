using Domain.Common;

namespace Domain.Entities {
	public class RolePermission : AbstractEntity {

		// Fields
		public Guid? AssignedBy { get; set; }
		public string AssignedByName { get; set; }

		// Foreign Keys
		public Guid RoleId { get; set; }
		public Guid PermissionId { get; set; }

		// Relationships
		public virtual Role Role { get; set; }
		public virtual Permission Permission { get; set; }
	}
}

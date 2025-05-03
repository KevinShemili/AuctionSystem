using Domain.Common;

namespace Domain.Entities {
	public class Role : EntityBase {

		// Fields
		public string Name { get; set; }
		public string Description { get; set; }

		// Relationships
		public virtual ICollection<User> Users { get; set; }
		public virtual ICollection<UserRole> UserRoles { get; set; }
		public virtual ICollection<Permission> Permissions { get; set; }
		public virtual ICollection<RolePermission> RolePermissions { get; set; }
	}
}

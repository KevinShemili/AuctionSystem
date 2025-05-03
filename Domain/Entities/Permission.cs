using Domain.Common;

namespace Domain.Entities {
	public class Permission : EntityBase {

		// Fields
		public string Key { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }

		// Relationships
		public virtual ICollection<Role> Roles { get; set; }
		public virtual ICollection<RolePermission> RolePermissions { get; set; }
	}
}

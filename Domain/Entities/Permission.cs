using Domain.Common;

namespace Domain.Entities {
	public class Permission : EntityBase {
		public required string Key { get; set; }
		public required string Name { get; set; }
		public string Description { get; set; }

		public virtual ICollection<Role> Roles { get; set; }
		public virtual ICollection<RolePermission> RolePermissions { get; set; }
	}
}

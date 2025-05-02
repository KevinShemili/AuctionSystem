using Domain.Common;

namespace Domain.Entities {
	public class RolePermission : EntityBase {
		public int RoleId { get; set; }
		public int PermissionId { get; set; }
		public Guid? AssignedBy { get; set; }
		public string AssignedByName { get; set; }

		public virtual Role Role { get; set; }
		public virtual Permission Permission { get; set; }
	}
}

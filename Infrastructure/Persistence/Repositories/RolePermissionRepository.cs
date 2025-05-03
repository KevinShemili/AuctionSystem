using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository {
		public RolePermissionRepository(DatabaseContext context) : base(context) {
		}
	}
}

using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository {
		public PermissionRepository(DatabaseContext context) : base(context) {
		}
	}
}

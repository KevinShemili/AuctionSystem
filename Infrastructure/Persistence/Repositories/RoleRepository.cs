using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class RoleRepository : BaseRepository<Role>, IRoleRepository {
		public RoleRepository(DatabaseContext context) : base(context) {
		}
	}
}

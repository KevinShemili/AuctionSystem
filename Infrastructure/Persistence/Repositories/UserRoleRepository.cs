using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class UserRoleRepository : BaseRepository<UserRole>, IUserRoleRepository {
		public UserRoleRepository(DatabaseContext context) : base(context) {
		}
	}
}

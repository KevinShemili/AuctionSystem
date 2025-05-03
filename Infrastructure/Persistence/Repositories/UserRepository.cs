using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class UserRepository : BaseRepository<User>, IUserRepository {
		public UserRepository(DatabaseContext context) : base(context) {
		}
	}
}

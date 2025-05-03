using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class UserTokenRepository : BaseRepository<UserToken>, IUserTokenRepository {
		public UserTokenRepository(DatabaseContext context) : base(context) {
		}
	}
}

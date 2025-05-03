using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class AuthenticationTokenRepository : BaseRepository<AuthenticationToken>, IAuthenticationTokenRepository {
		public AuthenticationTokenRepository(DatabaseContext context) : base(context) {
		}
	}
}

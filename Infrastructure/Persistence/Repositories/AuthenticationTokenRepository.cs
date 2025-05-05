using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class AuthenticationTokenRepository : BaseRepository<AuthenticationToken>, IAuthenticationTokenRepository {

		public AuthenticationTokenRepository(DatabaseContext context) : base(context) {
		}

		public async Task<AuthenticationToken> GetByUserID(Guid userId, CancellationToken cancellationToken = default) {
			var refreshToken = await SetTracking().FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

			return refreshToken;
		}
	}
}

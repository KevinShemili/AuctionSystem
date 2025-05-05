using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class UserRepository : BaseRepository<User>, IUserRepository {
		public UserRepository(DatabaseContext context) : base(context) {
		}

		public async Task<bool> DoesEmailExistAsync(string email, CancellationToken cancellationToken = default) {

			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentNullException(nameof(email));

			var emailBool = await SetNoTracking().AnyAsync(x => x.Email == email, cancellationToken);

			return emailBool;
		}

		public async Task<User> GetByEmailNoTrackingAsync(string email, CancellationToken cancellationToken = default) {

			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentNullException(nameof(email));

			var user = await SetNoTracking().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

			return user;
		}

		public async Task<User> GetUserWithAuthenticationTokensAsync(string email, CancellationToken cancellationToken = default) {
			var user = await SetTracking().Include(x => x.AuthenticationTokens)
										  .FirstOrDefaultAsync(x => x.Email == email, cancellationToken: cancellationToken);

			return user;
		}

		public async Task<User> GetUserWithAuthenticationTokensNoTrackingAsync(string email, CancellationToken cancellationToken = default) {
			var user = await SetNoTracking().Include(x => x.AuthenticationTokens)
											.FirstOrDefaultAsync(x => x.Email == email, cancellationToken: cancellationToken);

			return user;
		}

		public async Task<User> GetUserWithRolesAndPermissionsNoTrackingAsync(string email, CancellationToken cancellationToken = default) {
			var user = await SetNoTracking().Include(x => x.Roles)
											.ThenInclude(x => x.Permissions)
											.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

			return user;
		}

		public async Task<User> GetUserWithTokensAsync(string email, CancellationToken cancellationToken = default) {
			var user = await SetTracking().Include(x => x.UserTokens)
										  .Where(x => x.Email == email)
										  .FirstOrDefaultAsync(cancellationToken: cancellationToken);

			return user;
		}
	}
}

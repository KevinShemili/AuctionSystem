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

		public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) {

			if (string.IsNullOrWhiteSpace(email))
				throw new ArgumentNullException(nameof(email));

			var user = await SetNoTracking().FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

			return user;
		}
	}
}

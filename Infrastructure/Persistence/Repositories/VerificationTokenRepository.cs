using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;

namespace Infrastructure.Persistence.Repositories {
	public class VerificationTokenRepository : BaseRepository<VerificationToken>, IVerificationTokenRepository {
		public VerificationTokenRepository(DatabaseContext context) : base(context) {
		}
	}
}

using Application.Contracts.Repositories.UnitOfWork;

namespace Infrastructure.Persistence.Repositories {
	public class UnitOfWork : IUnitOfWork {

		private readonly DatabaseContext _context;

		public UnitOfWork(DatabaseContext context) {
			_context = context;
		}

		public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) {

			_ = await _context.SaveChangesAsync(cancellationToken);

			return true;
		}
	}
}

using Application.Contracts.Repositories.Common;
using Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Common {
	public abstract class BaseRepository<T> : IRepository<T> where T : AbstractEntity {

		protected readonly DatabaseContext _context;

		protected BaseRepository(DatabaseContext context) {
			_context = context;
		}

		public async Task<T> CreateAsync(T entity, bool commitChanges = false,
			CancellationToken cancellationToken = default) {

			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			entity.DateCreated = DateTime.UtcNow;

			_ = await _context.Set<T>().AddAsync(entity, cancellationToken);

			if (commitChanges is true)
				_ = await _context.SaveChangesAsync(cancellationToken);

			return entity;
		}

		public async Task<T> UpdateAsync(T entity, bool commitChanges = false,
			CancellationToken cancellationToken = default) {

			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			entity.DateUpdated = DateTime.UtcNow;

			_ = _context.Set<T>().Update(entity);

			if (commitChanges is true)
				_ = await _context.SaveChangesAsync(cancellationToken);

			return entity;
		}

		public async Task<bool> DeleteAsync(T entity, bool commitChanges = false,
			CancellationToken cancellationToken = default) {

			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			entity.IsDeleted = true;

			_ = await UpdateAsync(entity, commitChanges, cancellationToken);

			return true;
		}

		public async Task<bool> HardDeleteAsync(T entity, bool commitChanges = false,
			CancellationToken cancellationToken = default) {

			if (entity is null)
				throw new ArgumentNullException(nameof(entity));

			_context.Set<T>().Remove(entity);

			if (commitChanges is true)
				_ = await _context.SaveChangesAsync(cancellationToken);

			return true;
		}

		public IQueryable<T> SetNoTracking() {
			return _context.Set<T>().AsNoTracking();
		}

		public DbSet<T> SetTracking() {
			return _context.Set<T>();
		}

		public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) {

			_ = await _context.SaveChangesAsync(cancellationToken);

			return true;
		}
	}
}
using Application.Contracts.Repositories.Common;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

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

		[ExcludeFromCodeCoverage]
		public IQueryable<T> SetNoTracking() {
			return _context.Set<T>().AsNoTracking();
		}

		[ExcludeFromCodeCoverage]
		public DbSet<T> SetTracking() {
			return _context.Set<T>();
		}

		public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) {

			if (id == Guid.Empty)
				throw new ArgumentNullException(nameof(id));

			var entity = await SetTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			return entity;
		}

		public async Task<T> GetByIdNoTrackingAsync(Guid id, CancellationToken cancellationToken = default) {

			if (id == Guid.Empty)
				throw new ArgumentNullException(nameof(id));

			var entity = await SetNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

			return entity;
		}
	}
}
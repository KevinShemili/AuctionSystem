using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Application.Contracts.Repositories.Common {
	public interface IRepository<T> where T : AbstractEntity {

		Task<EntityEntry<T>> CreateAsync(T entity, bool commitChanges = false, CancellationToken cancellationToken = default);

		Task<bool> DeleteAsync(T entity, bool commitChanges = false, CancellationToken cancellationToken = default);

		Task<bool> HardDeleteAsync(T entity, bool commitChanges = false, CancellationToken cancellationToken = default);

		Task<T> UpdateAsync(T entity, bool commitChanges = false, CancellationToken cancellationToken = default);

		IQueryable<T> SetNoTracking();

		DbSet<T> SetTracking();

		Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}

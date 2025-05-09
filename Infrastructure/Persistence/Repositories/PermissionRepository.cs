using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository {
		public PermissionRepository(DatabaseContext context) : base(context) {
		}

		public async Task<bool> DoPermissionsExistAsync(List<Guid> permissionIds, CancellationToken cancellationToken = default) {

			var existingIds = await SetNoTracking().Where(x => permissionIds.Contains(x.Id))
												   .Select(x => x.Id)
												   .ToListAsync(cancellationToken: cancellationToken);

			return existingIds.Count == permissionIds.Count;
		}
	}
}

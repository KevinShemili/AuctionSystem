using Application.Contracts.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Repositories.Common;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories {
	public class RoleRepository : BaseRepository<Role>, IRoleRepository {
		public RoleRepository(DatabaseContext context) : base(context) {
		}

		public async Task<bool> DoRolesExistAsync(List<Guid> roleIds, CancellationToken cancellationToken = default) {
			var existingIds = await SetNoTracking().Where(x => roleIds.Contains(x.Id))
												   .Select(x => x.Id)
												   .ToListAsync(cancellationToken: cancellationToken);

			return existingIds.Count == roleIds.Count;
		}

		public async Task<Role> GetRoleWithPermissionsNoTrackingAsync(Guid roleId, CancellationToken cancellationToken = default) {

			var role = await SetTracking().Include(x => x.Permissions)
										  .Where(x => x.Id == roleId)
										  .FirstOrDefaultAsync(cancellationToken: cancellationToken);

			return role;
		}

		public async Task<Role> GetRoleWithRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default) {

			var role = await SetTracking().Include(x => x.RolePermissions)
										  .Where(x => x.Id == roleId)
										  .FirstOrDefaultAsync(cancellationToken: cancellationToken);

			return role;
		}

		public IQueryable<Role> SetNoTracking(string filter) {

			if (string.IsNullOrWhiteSpace(filter))
				return SetNoTracking();

			filter = filter.ToLower();

			return SetNoTracking().Where(x => x.Description.ToLower().Contains(filter) ||
										 x.Name.ToLower().Contains(filter));
		}
	}
}

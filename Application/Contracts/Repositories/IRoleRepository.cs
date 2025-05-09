using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IRoleRepository : IRepository<Role> {
		Task<bool> DoRolesExistAsync(List<Guid> roleIds, CancellationToken cancellationToken = default);
		Task<Role> GetRoleWithRolePermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
		Task<Role> GetRoleWithPermissionsNoTrackingAsync(Guid roleId, CancellationToken cancellationToken = default);
	}
}

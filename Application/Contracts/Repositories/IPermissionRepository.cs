using Application.Contracts.Repositories.Common;
using Domain.Entities;

namespace Application.Contracts.Repositories {
	public interface IPermissionRepository : IRepository<Permission> {
		Task<bool> DoPermissionsExistAsync(List<Guid> permissionIds, CancellationToken cancellationToken = default);
	}
}

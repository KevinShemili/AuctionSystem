using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization {

	// Market class that carries the name of the permssion to check
	// IAuthorizationRequirement is a marker interface, which
	// .NET uses for PermissionAuthorizationHandler<>
	public class PermissionRequirement : IAuthorizationRequirement {

		public string Permission { get; }

		// Capture string passed in from the [Authorize(Policy="")] annotation
		public PermissionRequirement(string permission) {
			Permission = permission;
		}
	}
}

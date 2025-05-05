using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization {
	public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement> {
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
			PermissionRequirement requirement) {
			if (context.User == null || !context.User.Identity!.IsAuthenticated) {
				return Task.CompletedTask;
			}

			var permissionsClaim = context.User.Claims.Where(x => x.Type == "Permissions")
													  .ToList();

			if (permissionsClaim is not null &&
				permissionsClaim.Select(x => x.Value).Contains(requirement.Permission)) {
				context.Succeed(requirement);
			}

			return Task.CompletedTask;
		}
	}
}

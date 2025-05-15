using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization {

	// The class actually overrides the check that .NET automatically performs when performing authorization.
	// By inheriting from AuthorizationHandler<TRequirement> and overriding the method.
	public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement> {
		protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement) {

			// If no user in the context or they are un-authenticated, return immediately.
			if (context.User is null || context.User.Identity!.IsAuthenticated is false)
				return Task.CompletedTask;

			// From the JWT get all claims of type "Permissions"
			var permissionsClaim = context.User.Claims.Where(x => x.Type == "Permissions")
													  .ToList();

			// If the user's JWT has the required claim
			if (permissionsClaim is not null && permissionsClaim.Select(x => x.Value).Contains(requirement.Permission))
				context.Succeed(requirement); // Indicate success

			return Task.CompletedTask;
		}
	}
}

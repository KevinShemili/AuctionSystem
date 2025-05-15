using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authorization {

	// By inheriting from DefaultAuthorizationPolicyProvider, giving us access to the [Authorize(Policy="")] annotation.
	// We override GetPolicyAsync so that whenever the framework sees a policy name it hasn’t seen before,
	// We dynamically build one that contains our PermissionRequirement for that policy name
	// This would otherwise be a static list of policies.
	public class PermissionAuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider {

		public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options) {
		}

		public async override Task<AuthorizationPolicy> GetPolicyAsync(string policyName) {

			// Check if the policy already is registered
			var policy = await base.GetPolicyAsync(policyName);

			if (policy is not null)
				return policy;

			// If no matching policy was found, build new policy
			return new AuthorizationPolicyBuilder()
				.AddRequirements(new PermissionRequirement(policyName))
				.Build();
		}
	}
}

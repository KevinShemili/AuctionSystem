using Hangfire.Dashboard;

namespace Infrastructure.Hangfire {
	public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter {
		public bool Authorize(DashboardContext context) => true;
	}
}

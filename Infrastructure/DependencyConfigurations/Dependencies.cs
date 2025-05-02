using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyConfigurations {
	public static class Dependencies {

		public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services,
			IConfiguration configuration) {

			services.ConfigureDatabaseConnection(configuration);


			return services;
		}

		private static void ConfigureDatabaseConnection(this IServiceCollection services,
			IConfiguration configuration) {

			var connString = configuration.GetConnectionString("DbConnection");

			services.AddDbContext<DatabaseContext>(options =>
				options.UseNpgsql(connString, b => b.MigrationsAssembly("Infrastructure")));
		}
	}
}

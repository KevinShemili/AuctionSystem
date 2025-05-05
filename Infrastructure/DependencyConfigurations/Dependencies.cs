using Application.Common.EmailService;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Infrastructure.Email;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyConfigurations {
	public static class Dependencies {

		public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration) {

			services.ConfigureDatabaseConnection(configuration);
			services.ConfigureScopedServices();

			return services;
		}

		private static void ConfigureDatabaseConnection(this IServiceCollection services, IConfiguration configuration) {

			var connString = configuration.GetConnectionString("DockerConnectionString");

			services.AddDbContext<DatabaseContext>(options =>
				options.UseNpgsql(connString, b => b.MigrationsAssembly("Infrastructure")));
		}

		private static void ConfigureScopedServices(this IServiceCollection services) {
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddScoped<IAuthenticationTokenRepository, AuthenticationTokenRepository>();
			services.AddScoped<IUserTokenRepository, UserTokenRepository>();
			services.AddScoped<IPermissionRepository, PermissionRepository>();
			services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
			services.AddScoped<IRoleRepository, RoleRepository>();
			services.AddScoped<IUserRoleRepository, UserRoleRepository>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IUnitOfWork, UnitOfWork>();
		}
	}
}

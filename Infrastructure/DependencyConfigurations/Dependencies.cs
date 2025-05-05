using Application.Common.EmailService;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Infrastructure.Authorization;
using Infrastructure.Email;
using Infrastructure.Persistence;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DependencyConfigurations {
	public static class Dependencies {

		public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration) {

			services.ConfigureDatabaseConnection(configuration);
			services.ConfigureScopedServices();
			services.ConfigureAuthorizationPolicy();

			return services;
		}

		private static void ConfigureDatabaseConnection(this IServiceCollection services, IConfiguration configuration) {

			var connString = configuration.GetConnectionString("DBString");

			services.AddDbContext<DatabaseContext>(options => {
				options.UseNpgsql(connString, b => b.MigrationsAssembly("Infrastructure"));
				options.LogTo(Console.WriteLine, LogLevel.Information);
			});
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

		private static void ConfigureAuthorizationPolicy(this IServiceCollection services) {
			services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
			services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
		}
	}
}

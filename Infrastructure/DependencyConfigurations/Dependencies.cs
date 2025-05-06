using Application.Common.Broadcast;
using Application.Common.EmailService;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Infrastructure.Authorization;
using Infrastructure.Broadcast;
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
			services.ConfigureSingletonServices();
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
			services.AddScoped<IAuctionRepository, AuctionRepository>();
			services.AddScoped<IAuctionImageRepostiory, AuctionImageRepostiory>();
			services.AddScoped<IBidRepository, BidRepository>();
			services.AddScoped<IWalletRepository, WalletRepository>();
			services.AddScoped<IWalletTransactionRepository, WalletTransactionRepository>();
		}

		private static void ConfigureSingletonServices(this IServiceCollection services) {
			services.AddSingleton<IBroadcastService, BroadcastService>();
		}

		private static void ConfigureAuthorizationPolicy(this IServiceCollection services) {
			services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
			services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
		}
	}
}

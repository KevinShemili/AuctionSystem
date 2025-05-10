using Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Environment {
	public class ContainerFactory<T> : WebApplicationFactory<T>, IAsyncLifetime
		where T : class {

		private PostgreSqlContainer _dbContainer;
		public string ConnectionString => _dbContainer.GetConnectionString();

		public async Task InitializeAsync() {

			_dbContainer = new PostgreSqlBuilder()
				.WithImage("postgres:17.4")
				.WithDatabase("IntegrationTestDB")
				.WithUsername("postgres")
				.WithPassword("postgres")
				.WithCleanUp(true)
				.Build();

			await _dbContainer.StartAsync();
		}

		async Task IAsyncLifetime.DisposeAsync() {
			if (_dbContainer is not null)
				await _dbContainer.DisposeAsync().AsTask();
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder) {

			builder.ConfigureAppConfiguration((context, config) => {
				config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
				config.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true);
				config.AddEnvironmentVariables();
			});

			builder.ConfigureTestServices(services => {

				// Remove the real database registration
				var context = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<DatabaseContext>));

				if (context != null)
					services.Remove(context);

				// Point DatabaseContext to test container
				services.AddDbContext<DatabaseContext>(opts =>
					opts.UseNpgsql(ConnectionString,
						sql => sql.MigrationsAssembly(typeof(DatabaseContext).Assembly.FullName))
				);

				// Apply schema to container
				var serviceProvider = services.BuildServiceProvider();
				using var scope = serviceProvider.CreateScope();
				var database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
				database.Database.Migrate();
			});
		}
	}
}

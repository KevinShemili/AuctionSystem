using Infrastructure.Persistence;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Environment {
	public class BaseIntegrationTest : IClassFixture<ContainerFactory<Program>> {

		protected readonly IServiceScope _serviceScope;
		protected readonly IMediator _mediator;
		protected readonly DatabaseContext _databaseContext;
		protected readonly HttpClient _client;

		protected BaseIntegrationTest(ContainerFactory<Program> factory) {

			_client = factory.CreateClient();
			_serviceScope = factory.Services.CreateScope();
			_mediator = _serviceScope.ServiceProvider.GetRequiredService<IMediator>();
			_databaseContext = _serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
		}
	}
}

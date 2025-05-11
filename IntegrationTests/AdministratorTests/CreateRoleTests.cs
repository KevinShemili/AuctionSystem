using Application.UseCases.Administrator.Commands;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class CreateRoleTests : BaseIntegrationTest {

		public CreateRoleTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task CreateRole_HappyPath() {

			// Arrange
			var command = new CreateRoleCommand {
				Name = "X",
				Description = "X"
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotEqual(Guid.Empty, result.Value);

			var role = await _databaseContext.Roles.AsNoTracking()
												   .FirstOrDefaultAsync(r => r.Id == result.Value);

			Assert.NotNull(role);
		}
	}
}

using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using IntegrationTests.Environment;

namespace IntegrationTests.AdministratorTests {
	public class ViewPermissionTests : BaseIntegrationTest {

		public ViewPermissionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ViewPermission_HappyPath() {

			// Arrange
			var permission = new Permission {
				Key = "PermissionX",
				Name = "PermissionX",
			};

			_ = await _databaseContext.Permissions.AddAsync(permission);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new ViewPermissionQuery {
				PermissionId = permission.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var permissionDTO = result.Value;

			// Validate role
			Assert.Equal(permission.Id, permissionDTO.Id);
			Assert.Equal(permission.Name, permissionDTO.Name);
		}
	}
}

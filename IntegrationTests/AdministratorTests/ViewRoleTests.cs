using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class ViewRoleTests : BaseIntegrationTest {

		public ViewRoleTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ViewRole_HappyPath() {

			// Arrange
			var permission1 = await _databaseContext.Permissions.FirstOrDefaultAsync(x => x.Id == SeedData.DeleteAuction.Id);
			var role = new Role {
				Name = "RoleX",
				Permissions = new List<Permission> {
					permission1
				}
			};

			_ = await _databaseContext.Roles.AddAsync(role);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new ViewRoleQuery {
				RoleId = role.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var roleDto = result.Value;

			// Validate role
			Assert.Equal(role.Id, roleDto.Id);
			Assert.Equal(role.Name, roleDto.Name);

			// Verify permission is returned
			Assert.NotNull(roleDto.Permissions);
			var permissionsList = roleDto.Permissions.ToList();

			Assert.Single(permissionsList);
			var permissionDTO = permissionsList.FirstOrDefault();
			Assert.NotNull(permissionDTO);
		}
	}
}

using Application.UseCases.Administrator.Commands;
using Domain.Entities;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class AssignPermissionTests : BaseIntegrationTest {

		public AssignPermissionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task AssignPermission_HappyPath() {

			// Arrange
			var permission1 = new Permission { Id = Guid.NewGuid(), Key = "X", Name = "X", Description = "X" };
			var permission2 = new Permission { Id = Guid.NewGuid(), Key = "X", Name = "X", Description = "X" };
			_ = await _databaseContext.Permissions.AddAsync(permission1);
			_ = await _databaseContext.Permissions.AddAsync(permission2);

			var role = new Role { Id = Guid.NewGuid(), Name = "X", Description = "X" };
			_ = await _databaseContext.Roles.AddAsync(role);

			_ = await _databaseContext.SaveChangesAsync();

			var actingAdmin = new User {
				Id = Guid.NewGuid(),
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				IsAdministrator = true,
			};

			var command = new AssignPermissionCommand {
				AdminId = actingAdmin.Id,
				RoleId = role.Id,
				PermissionIds = new List<Guid> { permission1.Id, permission2.Id }
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);

			var updatedRole = await _databaseContext.Roles.AsNoTracking()
														  .Include(x => x.Permissions)
														  .FirstOrDefaultAsync(x => x.Id == role.Id);

			Assert.NotNull(updatedRole);
			Assert.Equal(2, updatedRole.Permissions.Count);
			Assert.Contains(updatedRole.Permissions, x => x.Id == permission1.Id);
			Assert.Contains(updatedRole.Permissions, x => x.Id == permission2.Id);
		}
	}
}
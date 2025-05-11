using Application.UseCases.Administrator.Commands;
using Domain.Entities;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class AssignRoleTests : BaseIntegrationTest {

		public AssignRoleTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task AssignRole_HappyPath() {

			// Arrange
			var role1 = new Role { Id = Guid.NewGuid(), Name = "X", Description = "X" };
			var role2 = new Role { Id = Guid.NewGuid(), Name = "X", Description = "X" };
			_ = await _databaseContext.Roles.AddAsync(role1);
			_ = await _databaseContext.Roles.AddAsync(role2);

			var targetAdmin = new User {
				Id = Guid.NewGuid(),
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				IsAdministrator = true,
			};

			var actingAdmin = new User {
				Id = Guid.NewGuid(),
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				IsAdministrator = true,
			};

			_ = await _databaseContext.Users.AddAsync(targetAdmin);
			_ = await _databaseContext.Users.AddAsync(actingAdmin);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new AssignRoleCommand {
				AdminId = actingAdmin.Id,
				UserId = targetAdmin.Id,
				RoleIds = new List<Guid> { role1.Id, role2.Id }
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);

			var updatedTargetAdmin = await _databaseContext.Users.AsNoTracking()
																 .Include(x => x.Roles)
																	.ThenInclude(x => x.UserRoles)
																 .FirstOrDefaultAsync(x => x.Id == targetAdmin.Id);

			Assert.NotNull(updatedTargetAdmin);
			Assert.Equal(2, updatedTargetAdmin.Roles.Count);
			Assert.Contains(updatedTargetAdmin.Roles, x => x.Id == role1.Id);
			Assert.Contains(updatedTargetAdmin.Roles, x => x.Id == role2.Id);
		}
	}
}

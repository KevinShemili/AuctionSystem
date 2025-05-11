using Application.UseCases.Administrator.Commands;
using Domain.Entities;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class CreateAdminTests : BaseIntegrationTest {

		public CreateAdminTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task CreateAdmin_HappyPath() {

			// Arrange
			var email = $"{Guid.NewGuid()}@mail.com";
			var role1 = new Role { Id = Guid.NewGuid(), Name = "X", };
			var role2 = new Role { Id = Guid.NewGuid(), Name = "X" };

			_ = await _databaseContext.Roles.AddAsync(role1);
			_ = await _databaseContext.Roles.AddAsync(role2);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new CreateAdminCommand {
				FirstName = "X",
				LastName = "X",
				Email = email,
				Roles = new List<Guid> { role1.Id, role2.Id }
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess, "Expected command to succeed");

			var user = await _databaseContext.Users.AsNoTracking()
												   .Include(u => u.UserRoles)
												   .FirstOrDefaultAsync(u => u.Email == email);

			Assert.NotNull(user);
			Assert.Equal(email, user.Email);
			Assert.True(user.IsAdministrator);
			Assert.True(user.IsEmailVerified);
			Assert.Equal(2, user.UserRoles.Count);
			Assert.Contains(user.UserRoles, ur => ur.RoleId == role1.Id);
			Assert.Contains(user.UserRoles, ur => ur.RoleId == role2.Id);
		}
	}
}

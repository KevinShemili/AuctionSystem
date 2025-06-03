using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using IntegrationTests.Environment;

namespace IntegrationTests.AdministratorTests {
	public class ViewRolesTests : BaseIntegrationTest {

		public ViewRolesTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ViewRoles_HappyPath() {

			// Arrange
			var role1 = new Role {
				Name = "Role1"
			};

			var role2 = new Role {
				Name = "Role2"
			};

			await _databaseContext.Roles.AddRangeAsync(role1, role2);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new ViewRolesQuery {
				PageNumber = 1,
				PageSize = 10,
				Filter = string.Empty,
				SortBy = nameof(Role.Name),
				SortDesc = false
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var pagedResult = result.Value;

			// Verify pagination data
			Assert.Equal(1, pagedResult.PageNumber);
			Assert.Equal(10, pagedResult.PageSize);
			Assert.Equal(3, pagedResult.TotalRecords);

			Assert.Equal(3, pagedResult.Items.Count); // 2 added + 1 sseded

			var namesReturned = pagedResult.Items.Select(dto => dto.Name).ToList();
			Assert.Contains("Role1", namesReturned);
			Assert.Contains("Role2", namesReturned);
		}
	}
}

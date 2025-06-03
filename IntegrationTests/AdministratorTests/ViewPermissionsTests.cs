using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using IntegrationTests.Environment;

namespace IntegrationTests.AdministratorTests {
	public class ViewPermissionsTests : BaseIntegrationTest {

		public ViewPermissionsTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ViewPermissions_HappyPath() {

			// Arrange
			var permission1 = new Permission {
				Key = "CreateX",
				Name = "CreateX",
			};
			var permission2 = new Permission {
				Key = "DeleteX",
				Name = "DeleteX",
			};
			var permission3 = new Permission {
				Key = "UpdateX",
				Name = "UpdateX",
			};

			await _databaseContext.Permissions.AddRangeAsync(permission1, permission2, permission3);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new ViewPermissionsQuery {
				PageNumber = 1,
				PageSize = 15,
				Filter = string.Empty,
				SortBy = nameof(Permission.Name),
				SortDesc = false
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var pagedResult = result.Value;

			// Verify pagination data
			Assert.Equal(1, pagedResult.PageNumber);
			Assert.Equal(15, pagedResult.PageSize);
			Assert.Equal(13, pagedResult.TotalRecords); // 10 seeeded + 3 added now
			Assert.Equal(13, pagedResult.Items.Count);

			var namesReturned = pagedResult.Items.Select(dto => dto.Name).ToList();
			Assert.Contains("CreateX", namesReturned);
			Assert.Contains("DeleteX", namesReturned);
			Assert.Contains("UpdateX", namesReturned);
		}
	}
}

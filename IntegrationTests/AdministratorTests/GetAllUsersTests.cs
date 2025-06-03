using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using IntegrationTests.Environment;

namespace IntegrationTests.AdministratorTests {
	public class GetAllUsersTests : BaseIntegrationTest {

		public GetAllUsersTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task GetAllUsers_HappyPath() {

			// Arrange
			var user1 = new User {
				FirstName = "X1",
				LastName = "X1",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X1",
				PasswordSalt = "X1",
				IsAdministrator = false,
				IsBlocked = false
			};

			var user2 = new User {
				FirstName = "X2",
				LastName = "X2",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X2",
				PasswordSalt = "X2",
				IsAdministrator = true,
				IsBlocked = false
			};

			var user3 = new User {
				FirstName = "X3",
				LastName = "X3",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X3",
				PasswordSalt = "X3",
				IsAdministrator = false,
				IsBlocked = true
			};

			await _databaseContext.Users.AddRangeAsync(user1, user2, user3);
			await _databaseContext.SaveChangesAsync();

			// Act
			var query = new GetAllUsersQuery {
				PageNumber = 1,
				PageSize = 10,
				Filter = string.Empty,
				SortBy = nameof(User.FirstName),
				SortDesc = false
			};

			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var pagedResult = result.Value;

			// Verify pagination
			Assert.Equal(1, pagedResult.PageNumber);
			Assert.Equal(10, pagedResult.PageSize);
			Assert.Equal(4, pagedResult.TotalRecords); // 3 added + 1 seed
			Assert.Equal(4, pagedResult.Items.Count);

			var returnedEmails = pagedResult.Items.Select(dto => dto.Email).ToList();
			Assert.Contains(user1.Email, returnedEmails);
			Assert.Contains(user2.Email, returnedEmails);
			Assert.Contains(user3.Email, returnedEmails);
		}
	}
}

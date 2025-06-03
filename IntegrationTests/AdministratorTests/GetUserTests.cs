using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.AdministratorTests {
	public class GetUserTests : BaseIntegrationTest {

		public GetUserTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task GetUser_HappyPath() {

			// Arrange
			var user = new User {
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X",
				PasswordSalt = "X",
				IsAdministrator = false,
				IsBlocked = false,
				Wallet = new Wallet {
					Balance = 500m,
					FrozenBalance = 50m,
					Transactions = new List<WalletTransaction>
					{
						new()
						{
							Amount = 200m,
							TransactionType = (int)WalletTransactionEnum.Debit
						},
						new()
						{
							Amount = 100m,
							TransactionType = (int)WalletTransactionEnum.Credit
						}
					}
				},
				Auctions = new List<Auction>() {
					new() {
						Name = "X",
						Description = "X",
						BaselinePrice = 150m,
						StartTime = DateTime.UtcNow,
						EndTime = DateTime.UtcNow.AddHours(2),
						Status = (int)AuctionStatusEnum.Active,
					}
				}
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new GetUserQuery {
				UserId = user.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var DTO = result.Value;

			// Verify basic details
			Assert.Equal(user.Id, DTO.Id);
			Assert.Equal(user.FirstName, DTO.FirstName);
			Assert.Equal(user.LastName, DTO.LastName);
			Assert.Equal(user.Email, DTO.Email);
			Assert.False(DTO.IsAdministrator);
			Assert.False(DTO.IsBlocked);

			// Verify wallet
			Assert.Equal(user.Wallet.Id, DTO.WalletId);
			Assert.Equal(user.Wallet.Balance, DTO.Balance);
			Assert.Equal(user.Wallet.FrozenBalance, DTO.FrozenBalance);

			// verify roles 
			Assert.Null(DTO.Roles);

			// Verify auctions
			Assert.NotNull(DTO.CreatedAuctions);
			Assert.Single(DTO.CreatedAuctions);
		}
	}
}

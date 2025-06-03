using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.AdministratorTests {
	public class ViewWalletTests : BaseIntegrationTest {

		public ViewWalletTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ViewWallet_HappyPath() {

			// Arrange
			var user = new User {
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X",
				PasswordSalt = "X",
				IsAdministrator = false,
				Wallet = new Wallet {
					Balance = 1200m,
					FrozenBalance = 100m,
					Transactions = new List<WalletTransaction>
					{
						new()
						{
							Amount = 300m,
							TransactionType = (int)WalletTransactionEnum.Credit
						},
						new()
						{
							Amount = 150m,
							TransactionType = (int)WalletTransactionEnum.Debit
						}
					}
				}
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new ViewWalletQuery {
				WalletId = user.Wallet.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var DTO = result.Value;

			// Verify wallet fields
			Assert.Equal(user.Wallet.Id, DTO.Id);
			Assert.Equal(user.Wallet.Balance, DTO.Balance);
			Assert.Equal(user.Wallet.FrozenBalance, DTO.FrozenBalance);

			// Verify transactions mapping
			Assert.NotNull(DTO.Transactions);
			Assert.Equal(2, DTO.Transactions.Count());

			var deposit = DTO.Transactions.FirstOrDefault(x => x.Amount == 300m && x.TransactionType == (int)WalletTransactionEnum.Credit);
			Assert.NotNull(deposit);

			var debit = DTO.Transactions.FirstOrDefault(x => x.Amount == 150m && x.TransactionType == (int)WalletTransactionEnum.Debit);
			Assert.NotNull(debit);
		}
	}
}

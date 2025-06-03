using Application.UseCases.Profile.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.ProfileTests {

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
					Balance = 500m,
					FrozenBalance = 50m,
					Transactions = new List<WalletTransaction>
					{
						new() {
							Amount = 100m,
							TransactionType = (int)WalletTransactionEnum.Credit
						},
						new() {
							Amount = 25m,
							TransactionType = (int)WalletTransactionEnum.Debit
						}
					}
				}
			};

			await _databaseContext.Users.AddAsync(user);
			await _databaseContext.SaveChangesAsync();

			var query = new ViewMyWalletQuery {
				UserId = user.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var walletDTO = result.Value;

			// Verify Id, balance & frozen balance
			Assert.Equal(user.Wallet.Id, walletDTO.Id);
			Assert.Equal(500m, walletDTO.Balance);
			Assert.Equal(50m, walletDTO.FrozenBalance);

			// Verify number of transactions
			Assert.NotNull(walletDTO.Transactions);
			Assert.Equal(2, walletDTO.Transactions.Count());

			// Make sure transactions are correct
			var creditTransaction = walletDTO.Transactions.FirstOrDefault(x => x.Amount == 100m && x.TransactionType == (int)WalletTransactionEnum.Credit);
			Assert.NotNull(creditTransaction);

			var debitTransaction = walletDTO.Transactions.FirstOrDefault(x => x.Amount == 25m && x.TransactionType == (int)WalletTransactionEnum.Debit);
			Assert.NotNull(debitTransaction);
		}
	}
}
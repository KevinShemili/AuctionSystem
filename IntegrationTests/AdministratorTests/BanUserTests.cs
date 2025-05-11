using Application.UseCases.Administrator.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class BanUserTests : BaseIntegrationTest {

		public BanUserTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task BanUser_HappyPath() {

			// Arrange
			var bidderId1 = Guid.NewGuid();
			var bidderAmount1 = 250m;

			var bidderId2 = Guid.NewGuid();
			var bidderAmount2 = 300m;

			var bidderId3 = Guid.NewGuid();
			var bidderAmount3 = 300m;

			var targetUser = new User {
				Id = Guid.NewGuid(),
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X",
				PasswordSalt = "X",
				Auctions = new List<Auction>() {
					new() {
						Id = Guid.NewGuid(),
						Name = "X",
						Description = "X",
						BaselinePrice = 200m,
						StartTime = DateTime.UtcNow,
						EndTime = DateTime.UtcNow.AddHours(2),
						Status = (int)AuctionStatusEnum.Active,
						Bids = new List<Bid>() {
							new() {
								Id = Guid.NewGuid(),
								Amount = bidderAmount1,
								Bidder = new User {
									Id = bidderId1,
									FirstName = "X",
									LastName = "X",
									Email = $"{Guid.NewGuid()}@mail.com",
									PasswordHash = "X",
									PasswordSalt = "X",
									Wallet = new Wallet {
										Balance = 1000m,
										FrozenBalance = bidderAmount1,
										Transactions = new List<WalletTransaction>()
									}
								}
							},
							new() {
								Id = Guid.NewGuid(),
								Amount = bidderAmount2,
								Bidder = new User {
									Id = bidderId2,
									FirstName = "X",
									LastName = "X",
									PasswordHash = "X",
									PasswordSalt = "X",
									Email = $"{Guid.NewGuid()}@mail.com",
									Wallet = new Wallet {
										Balance = 1000m,
										FrozenBalance = bidderAmount2,
										Transactions = new List<WalletTransaction>()
									}
								}
							}
						}
					},
					new() {
						Id = Guid.NewGuid(),
						Name = "X",
						Description = "X",
						BaselinePrice = 300m,
						StartTime = DateTime.UtcNow,
						EndTime = DateTime.UtcNow.AddHours(2),
						Status = (int)AuctionStatusEnum.Active,
						Bids = new List<Bid>() {
							new() {
								Id = Guid.NewGuid(),
								Amount = bidderAmount3,
								Bidder = new User {
									Id = bidderId3,
									FirstName = "X",
									LastName = "X",
									PasswordHash = "X",
									PasswordSalt = "X",
									Email = $"{Guid.NewGuid()}@mail.com",
									Wallet = new Wallet {
										Balance = 1000m,
										FrozenBalance = bidderAmount3,
										Transactions = new List<WalletTransaction>()
									}
								}
							}
						}
					},
				},
				Bids = new List<Bid>() {
					new() {
						Id = Guid.NewGuid(),
						Amount = 400m,
						Auction = new Auction {
							Id = Guid.NewGuid(),
							Name = "X",
							Description = "X",
							BaselinePrice = 200m,
							Status = (int)AuctionStatusEnum.Active,
							Seller = new User {
								Id = Guid.NewGuid(),
								Email = $"{Guid.NewGuid()}@mail.com",
								FirstName = "X",
								LastName = "X",
								PasswordHash = "X",
								PasswordSalt = "X"
							}
						}
					},
					new() {
						Id = Guid.NewGuid(),
						Amount = 400m,
						Auction = new Auction {
							Id = Guid.NewGuid(),
							Name = "X",
							Description = "X",
							BaselinePrice = 100m,
							Status = (int)AuctionStatusEnum.Active,
							Seller = new User {
								Id = Guid.NewGuid(),
								Email = $"{Guid.NewGuid()}@mail.com",
								FirstName = "X",
								LastName = "X",
								PasswordHash = "X",
								PasswordSalt = "X"
							}
						}
					},
				}
			};

			_ = await _databaseContext.Users.AddAsync(targetUser);
			_ = await _databaseContext.SaveChangesAsync(CancellationToken.None);


			var command = new BanUserCommand {
				UserId = targetUser.Id,
				Reason = "Policy Violation."
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var bannedUser = await _databaseContext.Users.AsNoTracking()
														  .IgnoreQueryFilters()
														  .Include(x => x.Bids)
														  .Include(x => x.Auctions)
															.ThenInclude(x => x.Bids)
															.ThenInclude(x => x.Bidder)
															.ThenInclude(x => x.Wallet)
															.ThenInclude(x => x.Transactions)
														  .FirstOrDefaultAsync(x => x.Id == targetUser.Id);

			Assert.NotNull(bannedUser);
			Assert.True(bannedUser.IsBlocked);

			// Check if all his bids are removed
			Assert.True(bannedUser.Bids.All(x => x.IsDeleted));

			// Check if all his auctions are removed
			Assert.True(bannedUser.Auctions.All(x => x.IsDeleted));

			// Check if bids on his auctions are removed & other bidders refunded
			Assert.True(bannedUser.Auctions.All(x => x.Bids.All(b => b.IsDeleted)));

			var biddersOnBannedUserAuctions = bannedUser.Auctions.SelectMany(x => x.Bids)
																 .Select(x => x.Bidder)
																 .ToList();

			foreach (var bidder in biddersOnBannedUserAuctions) {

				Assert.Equal(0m, bidder.Wallet.FrozenBalance);
				Assert.Single(bidder.Wallet.Transactions);
				Assert.Contains(bidder.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Unfreeze);
			}
		}
	}
}
using Application.UseCases.Administrator.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class ForceCloseAuctionTests : BaseIntegrationTest {

		public ForceCloseAuctionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ForceCloseAuction_ExistingBids_HappyPath() {

			// Arrange
			var auctionId = Guid.NewGuid();
			var bidAdmount1 = 350m;
			var bidAdmount2 = 250m;
			var bidderId1 = Guid.NewGuid();
			var bidderId2 = Guid.NewGuid();

			var auction = new Auction {
				Id = auctionId,
				Name = "X",
				Description = "X",
				BaselinePrice = 200m,
				StartTime = DateTime.UtcNow,
				EndTime = DateTime.UtcNow.AddHours(2),
				Status = (int)AuctionStatusEnum.Active,
				Seller = new User {
					Id = Guid.NewGuid(),
					FirstName = "X",
					LastName = "X",
					Email = $"{Guid.NewGuid()}@mail.com",
					PasswordHash = "X",
					PasswordSalt = "X",
					Wallet = new Wallet {
						Balance = 0m,
						FrozenBalance = 0m,
						Transactions = new List<WalletTransaction>()
					}
				},
				Bids = new List<Bid> {
					new () {
						Id = Guid.NewGuid(),
						Amount = bidAdmount1,
						Bidder = new User {
							Id = bidderId1,
							FirstName = "X",
							LastName = "X",
							Email = $"{Guid.NewGuid()}@mail.com",
							PasswordHash = "X",
							PasswordSalt = "X",
							Wallet = new Wallet {
								Balance = 1000m,
								FrozenBalance = bidAdmount1,
								Transactions = new List<WalletTransaction>()
							}
						}
					},
					new () {
						Id = Guid.NewGuid(),
						Amount = bidAdmount2,
						Bidder = new User {
							Id = bidderId2,
							FirstName = "X",
							LastName = "X",
							Email = $"{Guid.NewGuid()}@mail.com",
							PasswordHash = "X",
							PasswordSalt = "X",
							Wallet = new Wallet {
								Balance = 0m,
								FrozenBalance = bidAdmount2,
								Transactions = new List<WalletTransaction>()
							}
						}
					}
				}
			};

			var actingAdmin = new User {
				Id = Guid.NewGuid(),
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				IsAdministrator = true,
				PasswordHash = "X",
				PasswordSalt = "X"
			};

			_ = await _databaseContext.Auctions.AddAsync(auction);
			_ = await _databaseContext.Users.AddAsync(actingAdmin);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new ForceCloseAuctionCommand {
				AuctionId = auction.Id,
				AdminId = actingAdmin.Id,
				Reason = "Violation of terms"
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);

			var updatedAuction = await _databaseContext.Auctions.AsNoTracking()
																.Include(x => x.Bids)
																.FirstOrDefaultAsync(x => x.Id == auction.Id);

			Assert.NotNull(updatedAuction);
			Assert.Equal((int)AuctionStatusEnum.Ended, updatedAuction.Status);
			Assert.Equal(actingAdmin.Id, updatedAuction.ForceClosedBy);
			Assert.Empty(updatedAuction.Bids);

			var updatedBidder1 = await _databaseContext.Users.AsNoTracking()
															 .Include(u => u.Wallet)
																.ThenInclude(w => w.Transactions)
															 .FirstOrDefaultAsync(u => u.Id == bidderId1);

			Assert.NotNull(updatedBidder1);
			Assert.Equal(0m, updatedBidder1.Wallet.FrozenBalance);
			Assert.Single(updatedBidder1.Wallet.Transactions);
			Assert.Equal(bidAdmount1, updatedBidder1.Wallet.Transactions.FirstOrDefault().Amount);
			Assert.Equal((int)WalletTransactionEnum.Unfreeze, updatedBidder1.Wallet.Transactions.FirstOrDefault().TransactionType);

			var updatedBidder2 = await _databaseContext.Users.AsNoTracking()
															 .Include(u => u.Wallet)
																.ThenInclude(w => w.Transactions)
															 .FirstOrDefaultAsync(u => u.Id == bidderId2);

			Assert.NotNull(updatedBidder2);
			Assert.Equal(0m, updatedBidder2.Wallet.FrozenBalance);
			Assert.Single(updatedBidder2.Wallet.Transactions);
			Assert.Equal(bidAdmount2, updatedBidder2.Wallet.Transactions.FirstOrDefault().Amount);
			Assert.Equal((int)WalletTransactionEnum.Unfreeze, updatedBidder2.Wallet.Transactions.FirstOrDefault().TransactionType);
		}
	}
}
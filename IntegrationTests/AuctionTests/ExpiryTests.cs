using Application.UseCases.AutomaticExpiry;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.AuctionTests {

	public class ExpiryTests : BaseIntegrationTest {

		public ExpiryTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task AutomaticClose_HappyPath_NoBids() {

			// Arrange
			var auction = new Auction {
				Id = Guid.NewGuid(),
				Name = "X",
				Status = (int)AuctionStatusEnum.Active,
				StartTime = DateTime.UtcNow.AddMinutes(-5),
				EndTime = DateTime.UtcNow.AddMinutes(-2),
				Seller = new User {
					Id = Guid.NewGuid(),
					Email = $"{Guid.NewGuid()}@mail.com",
					FirstName = "X",
					LastName = "X",
					PasswordHash = "X",
					PasswordSalt = "X"
				}
			};

			_ = await _databaseContext.Auctions.AddAsync(auction);
			_ = await _databaseContext.SaveChangesAsync();

			// Act
			var auctionCloser = _serviceScope.ServiceProvider.GetRequiredService<IAuctionCloser>();
			await auctionCloser.AutomaticClose();

			// Assert
			var updatedAuction = await _databaseContext.Auctions.AsNoTracking().FirstOrDefaultAsync(a => a.Id == auction.Id);
			Assert.Equal((int)AuctionStatusEnum.Ended, updatedAuction.Status);
		}

		[Fact]
		public async Task AutomaticClose_HappyPath_SingleBid() {

			// Arrange
			var auctionId = Guid.NewGuid();
			var selledId = Guid.NewGuid();
			var bidId = Guid.NewGuid();
			var bidderId = Guid.NewGuid();
			var bidAmount = 200m;

			var auction = new Auction {
				Id = auctionId,
				Name = "X",
				Status = (int)AuctionStatusEnum.Active,
				StartTime = DateTime.UtcNow.AddMinutes(-5),
				EndTime = DateTime.UtcNow.AddMinutes(-2),
				Seller = new User {
					Id = selledId,
					Email = $"{Guid.NewGuid()}@mail.com",
					FirstName = "X",
					LastName = "X",
					PasswordHash = "X",
					PasswordSalt = "X",
					Wallet = new Wallet {
						Balance = 0m,
						FrozenBalance = 0m
					}
				},
				Bids = new List<Bid> {
					new() {
						Id = bidId,
						Amount = bidAmount,
						DateCreated = DateTime.UtcNow.AddHours(-1),
						Bidder = new User {
							Id = bidderId,
							Email = $"{Guid.NewGuid()}@mail.com",
							FirstName = "Bidder",
							LastName = "One",
							PasswordHash = "X",
							PasswordSalt = "X",
							Wallet = new Wallet {
								Balance = bidAmount,
								FrozenBalance = bidAmount,
							}
						}
					}
				},
			};

			_ = await _databaseContext.Auctions.AddAsync(auction);
			_ = await _databaseContext.SaveChangesAsync();

			// Act
			var auctionCloser = _serviceScope.ServiceProvider.GetRequiredService<IAuctionCloser>();
			await auctionCloser.AutomaticClose();

			// Assert
			var updatedAuction = await _databaseContext.Auctions.AsNoTracking().FirstOrDefaultAsync(a => a.Id == auction.Id);
			Assert.Equal((int)AuctionStatusEnum.Ended, updatedAuction.Status);

			var updatedBid = await _databaseContext.Bids.AsNoTracking().FirstOrDefaultAsync(b => b.Id == bidId);
			Assert.True(updatedBid.IsWinningBid);

			var bidder = await _databaseContext.Users.AsNoTracking()
													 .Include(x => x.Wallet)
														.ThenInclude(x => x.Transactions)
													 .FirstOrDefaultAsync(x => x.Id == bidderId);
			Assert.Equal(0m, bidder.Wallet.FrozenBalance);
			Assert.Equal(0m, bidder.Wallet.Balance);

			var seller = await _databaseContext.Users.AsNoTracking()
													 .Include(x => x.Wallet)
														.ThenInclude(x => x.Transactions)
													 .FirstOrDefaultAsync(x => x.Id == selledId);
			Assert.Equal(bidAmount, seller.Wallet.Balance);

			Assert.Equal(2, bidder.Wallet.Transactions.Count);
			Assert.Contains(bidder.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Unfreeze && x.Amount == bidAmount);
			Assert.Contains(bidder.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Debit && x.Amount == bidAmount);

			Assert.Single(seller.Wallet.Transactions);
			Assert.Contains(seller.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Credit && x.Amount == bidAmount);
		}

		[Fact]
		public async Task AutomaticClose_HappyPath_MultipleBids() {

			// Arrange
			var auctionId = Guid.NewGuid();
			var selledId = Guid.NewGuid();

			var secondHighestBidId = Guid.NewGuid();
			var secondHighestBidderId = Guid.NewGuid();
			var secondHighestAmount = 250m; // Second Highest

			var winnerBidId = Guid.NewGuid();
			var winnerBidderId = Guid.NewGuid();
			var highestAmount = 350m; // Highest

			var loserBidId = Guid.NewGuid();
			var loserBidderId = Guid.NewGuid();
			var loserBidAmount = 200m;

			var auction = new Auction {
				Id = auctionId,
				Name = "X",
				Status = (int)AuctionStatusEnum.Active,
				StartTime = DateTime.UtcNow.AddMinutes(-5),
				EndTime = DateTime.UtcNow.AddMinutes(-2),
				Seller = new User {
					Id = selledId,
					Email = $"{Guid.NewGuid()}@mail.com",
					FirstName = "X",
					LastName = "X",
					PasswordHash = "X",
					PasswordSalt = "X",
					Wallet = new Wallet {
						Balance = 0m,
						FrozenBalance = 0m
					}
				},
				Bids = new List<Bid> {
					new() {
						Id = secondHighestBidId,
						Amount = secondHighestAmount,
						DateCreated = DateTime.UtcNow.AddHours(-1),
						Bidder = new User {
							Id = secondHighestBidderId,
							Email = $"{Guid.NewGuid()}@mail.com",
							FirstName = "Bidder",
							LastName = "One",
							PasswordHash = "X",
							PasswordSalt = "X",
							Wallet = new Wallet {
								Balance = 1000m,
								FrozenBalance = secondHighestAmount,
							}
						}
					},
					new() {
						Id = winnerBidId,
						Amount = highestAmount,
						DateCreated = DateTime.UtcNow.AddHours(-1),
						Bidder = new User {
							Id = winnerBidderId,
							Email = $"{Guid.NewGuid()}@mail.com",
							FirstName = "Bidder",
							LastName = "Two",
							PasswordHash = "X",
							PasswordSalt = "X",
							Wallet = new Wallet {
								Balance = 1000m,
								FrozenBalance = highestAmount,
							}
						}
					},
					new() {
						Id = loserBidId,
						Amount = loserBidAmount,
						DateCreated = DateTime.UtcNow.AddHours(-1),
						Bidder = new User {
							Id = loserBidderId,
							Email = $"{Guid.NewGuid()}@mail.com",
							FirstName = "Bidder",
							LastName = "Three",
							PasswordHash = "X",
							PasswordSalt = "X",
							Wallet = new Wallet {
								Balance = 1000m,
								FrozenBalance = loserBidAmount,
							}
						}
					}
				},
			};

			_ = await _databaseContext.Auctions.AddAsync(auction);
			_ = await _databaseContext.SaveChangesAsync();

			// Act
			var auctionCloser = _serviceScope.ServiceProvider.GetRequiredService<IAuctionCloser>();
			await auctionCloser.AutomaticClose();

			// Assert
			var updatedAuction = await _databaseContext.Auctions.AsNoTracking()
																.Include(x => x.Bids)
																.FirstOrDefaultAsync(a => a.Id == auction.Id);

			Assert.Equal((int)AuctionStatusEnum.Ended, updatedAuction.Status);

			var winningBid = updatedAuction.Bids.FirstOrDefault(x => x.BidderId == winnerBidderId);
			Assert.True(winningBid.IsWinningBid);

			var users = await _databaseContext.Users.AsNoTracking()
													.Include(x => x.Wallet)
														.ThenInclude(x => x.Transactions)
													.Include(x => x.Bids)
													.ToListAsync();

			Assert.Equal(secondHighestAmount, users.FirstOrDefault(x => x.Id == selledId).Wallet.Balance);

			Assert.Equal(0, users.FirstOrDefault(x => x.Id == winnerBidderId).Wallet.FrozenBalance);
			Assert.Equal(1000m - 250m, users.FirstOrDefault(x => x.Id == winnerBidderId).Wallet.Balance);

			foreach (var bid in updatedAuction.Bids.Where(x => x.BidderId != winnerBidderId)) {

				var user = users.FirstOrDefault(x => x.Id == bid.BidderId);

				Assert.Equal(0m, user.Wallet.FrozenBalance);
				Assert.Equal(1000m, user.Wallet.Balance);
			}


			var winner = users.FirstOrDefault(x => x.Id == winnerBidderId);
			Assert.Equal(2, winner.Wallet.Transactions.Count);
			Assert.Contains(winner.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Unfreeze && x.Amount == highestAmount);
			Assert.Contains(winner.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Debit && x.Amount == secondHighestAmount);

			var second = users.FirstOrDefault(x => x.Id == secondHighestBidderId);
			Assert.Single(second.Wallet.Transactions);
			Assert.Contains(second.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Unfreeze && x.Amount == secondHighestAmount);

			var loser = users.FirstOrDefault(x => x.Id == loserBidderId);
			Assert.Single(loser.Wallet.Transactions);
			Assert.Contains(loser.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Unfreeze && x.Amount == loserBidAmount);

			var seller = users.FirstOrDefault(x => x.Id == selledId);
			Assert.Single(seller.Wallet.Transactions);
			Assert.Contains(seller.Wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Credit && x.Amount == secondHighestAmount);
		}
	}
}
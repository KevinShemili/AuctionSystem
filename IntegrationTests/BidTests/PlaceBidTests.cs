using Application.UseCases.Bidding.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.BidTests {
	public class PlaceBidTests : BaseIntegrationTest {
		public PlaceBidTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task PlaceBid_HappyPath_FirstBid() {

			// Arrange

			// Seller with auction
			// Bidder
			var auctionId = Guid.NewGuid();

			var seller = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordSalt = "X",
				PasswordHash = "X",
				Auctions = new List<Auction>() {
					new() {
						Id = auctionId,
						Name = "X",
						BaselinePrice = 100m,
						Status = (int)AuctionStatusEnum.Active,
						StartTime = DateTime.UtcNow,
						EndTime = DateTime.UtcNow.AddHours(2),
					}
				}
			};

			var bidder = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordSalt = "X",
				PasswordHash = "X",
				Wallet = new Wallet {
					Balance = 10000m,
					FrozenBalance = 0m,
				}
			};

			_ = await _databaseContext.Users.AddAsync(seller);
			_ = await _databaseContext.Users.AddAsync(bidder);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new PlaceBidCommand {
				AuctionId = auctionId,
				BidderId = bidder.Id,
				Amount = 150m
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);

			// Assert bid created
			var bid = await _databaseContext.Bids.Include(x => x.Bidder)
												 .FirstOrDefaultAsync(x => x.Id == result.Value);

			Assert.NotNull(bid);
			Assert.Equal(150m, bid.Amount);
			Assert.Equal(bidder.Id, bid.BidderId);

			// Assert balance frozen
			var updatedWallet = await _databaseContext.Wallets.Include(w => w.Transactions)
															  .FirstOrDefaultAsync(w => w.UserId == bidder.Id);

			Assert.Equal(150m, updatedWallet.FrozenBalance);
			Assert.Single(updatedWallet.Transactions);
			Assert.Equal((int)WalletTransactionEnum.Freeze, updatedWallet.Transactions.FirstOrDefault().TransactionType);
		}

		[Fact]
		public async Task PlaceBid_HappyPath_ExstingPreviousBid() {

			// Arrange

			// Seller with auction
			// Bidder with existing bid
			var auctionId = Guid.NewGuid();
			var bidderId = Guid.NewGuid();
			var bidId = Guid.NewGuid();

			var seller = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordSalt = "X",
				PasswordHash = "X",
				Auctions = new List<Auction>() {
					new() {
						Id = auctionId,
						Name = "X",
						BaselinePrice = 100m,
						Status = (int)AuctionStatusEnum.Active,
						StartTime = DateTime.UtcNow,
						EndTime = DateTime.UtcNow.AddHours(2),
						Bids = new List<Bid>() {
							new() {
								Id = bidId,
								BidderId = bidderId,
								Amount = 200m,
								IsWinningBid = false
							}
						}
					}
				}
			};

			var bidder = new User {
				Id = bidderId,
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordSalt = "X",
				PasswordHash = "X",
				Wallet = new Wallet {
					Balance = 10000m,
					FrozenBalance = 200m,
					Transactions = new List<WalletTransaction>() {
						new() {
							Amount = 200m,
							TransactionType = (int)WalletTransactionEnum.Freeze,
							BidId = bidId,
							DateCreated = DateTime.UtcNow
						}
					}
				}
			};


			_ = await _databaseContext.Users.AddAsync(seller);
			_ = await _databaseContext.Users.AddAsync(bidder);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new PlaceBidCommand {
				AuctionId = auctionId,
				BidderId = bidderId,
				Amount = 300m // + 100
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(bidId, result.Value);

			// Assert bid updated with new amount
			var updatedBid = await _databaseContext.Bids.FirstOrDefaultAsync(b => b.Id == bidId);
			Assert.Equal(300m, updatedBid.Amount);

			var wallet = await _databaseContext.Wallets.Include(w => w.Transactions)
													   .FirstOrDefaultAsync(w => w.UserId == bidderId);

			// Assert frozen balance updated
			Assert.Equal(300m, wallet.FrozenBalance);

			// Assert 2 transactions (1 previous freeze + new freeze)
			Assert.Equal(2, wallet.Transactions.Count);
			Assert.Contains(wallet.Transactions, x => x.TransactionType == (int)WalletTransactionEnum.Freeze && x.Amount == 100m);
		}
	}
}
using Application.Common.Tools.Time;
using Application.UseCases.Administrator.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.AdministratorTests {
	public class ViewAuctionAdminTests : BaseIntegrationTest {

		public ViewAuctionAdminTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ViewAuctionAdmin_HappyPath() {

			// Arrange
			var now = TruncateTime.ToMinute(DateTime.UtcNow);

			var auction = new Auction {
				Name = "Admin View Auction",
				Description = "Auction visible to admin",
				BaselinePrice = 500m,
				StartTime = now,
				EndTime = now.AddHours(3),
				Status = (int)AuctionStatusEnum.Active,
				Seller = new User {
					FirstName = "X",
					LastName = "X",
					Email = $"{Guid.NewGuid()}@mail.com",
					PasswordHash = "X",
					PasswordSalt = "X",
					IsAdministrator = false
				},
				Images = new List<AuctionImage>
				{
					new() { FilePath = "X.jpg" },
				},
				Bids = new List<Bid>
				{
					new()
					{
						Amount = 550m,
						Bidder = new User {
							FirstName = "X1",
							LastName = "X1",
							Email = $"{Guid.NewGuid()}@mail.com",
							PasswordHash = "X1",
							PasswordSalt = "X1",
							IsAdministrator = false
						},
						IsWinningBid = false
					},
					new()
					{
						Amount = 600m,
						Bidder = new User {
							FirstName = "X2",
							LastName = "X2",
							Email = $"{Guid.NewGuid()}@mail.com",
							PasswordHash = "X2",
							PasswordSalt = "X2",
							IsAdministrator = false
						},
						IsWinningBid = true
					}
				}
			};

			_ = await _databaseContext.Auctions.AddAsync(auction);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new ViewAuctionAdminQuery {
				AuctionId = auction.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var DTO = result.Value;

			Assert.Equal(auction.Id, DTO.Id);
			Assert.Equal(auction.Name, DTO.Name);
			Assert.Equal(auction.Description, DTO.Description);
			Assert.Equal(auction.BaselinePrice, DTO.BaselinePrice);
			Assert.Equal(auction.StartTime, DTO.StartTime);
			Assert.Equal(auction.EndTime, DTO.EndTime);
			Assert.Equal(auction.Status, DTO.Status);
			Assert.Equal(auction.Seller.FirstName, DTO.SellerFirstName);
			Assert.Equal(auction.Seller.LastName, DTO.SellerLastName);
			Assert.Equal(auction.Seller.Id, DTO.SellerId);

			Assert.NotNull(DTO.Bids);
			Assert.Equal(2, DTO.Bids.Count);

			var bid1 = DTO.Bids.FirstOrDefault(b => b.Amount == 550m);
			Assert.NotNull(bid1);

			var bid2 = DTO.Bids.FirstOrDefault(b => b.Amount == 600m);
			Assert.NotNull(bid2);
		}
	}
}
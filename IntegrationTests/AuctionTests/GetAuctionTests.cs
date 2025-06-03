using Application.Common.Tools.Time;
using Application.UseCases.Auctions.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.AuctionTests {
	public class GetAuctionTests : BaseIntegrationTest {

		public GetAuctionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task GetAuction_HappyPath() {

			// Arrange
			var now = TruncateTime.ToMinute(DateTime.UtcNow);

			var auction = new Auction {
				Name = "X",
				Description = "X",
				BaselinePrice = 300m,
				StartTime = now,
				EndTime = now.AddHours(2),
				Status = (int)AuctionStatusEnum.Active,
				Seller = new User {
					FirstName = "X",
					LastName = "X",
					Email = $"{Guid.NewGuid()}@mail.com",
					PasswordHash = "X",
					PasswordSalt = "X",
				},
				Images = new List<AuctionImage>
				{
					new() { FilePath = "X.jpg" }
				},
				Bids = new List<Bid>
				{
					new()
					{
						Amount = 320m,
						Bidder = new User {
							FirstName = "X",
							LastName = "X",
							Email = $"{Guid.NewGuid()}@mail.com",
							PasswordHash = "X",
							PasswordSalt = "X"
						}
					}
				}
			};

			_ = await _databaseContext.Auctions.AddAsync(auction);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new GetAuctionQuery {
				AuctionId = auction.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var DTO = result.Value;

			// Verify basic auction fields
			Assert.Equal(auction.Id, DTO.Id);
			Assert.Equal(auction.Name, DTO.Name);
			Assert.Equal(auction.Description, DTO.Description);
			Assert.Equal(auction.BaselinePrice, DTO.BaselinePrice);
			Assert.Equal(auction.StartTime, DTO.StartTime);
			Assert.Equal(auction.EndTime, DTO.EndTime);
			Assert.Equal(auction.Status, DTO.Status);

			// Verify seller information
			Assert.Equal(auction.Seller.FirstName, DTO.SellerFirstName);
			Assert.Equal(auction.Seller.LastName, DTO.SellerLastName);
			Assert.Equal(auction.Seller.Email, DTO.SellerEmail);

			// Verify single bid
			Assert.Single(DTO.Bidders);
		}
	}
}

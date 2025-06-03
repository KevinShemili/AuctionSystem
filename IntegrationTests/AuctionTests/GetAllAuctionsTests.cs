using Application.Common.Tools.Time;
using Application.UseCases.Auctions.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.AuctionTests {
	public class GetAllAuctionsTests : BaseIntegrationTest {

		public GetAllAuctionsTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task GetAllAuctions_HappyPath() {

			// Arrange
			// 2 Auctions:
			// 1 - Active
			// 1 - Inactive

			var now = TruncateTime.ToMinute(DateTime.UtcNow);

			var activeAuction = new Auction {
				Name = "X",
				Description = "X",
				Status = (int)AuctionStatusEnum.Active,
				BaselinePrice = 150m,
				StartTime = now,
				EndTime = now.AddHours(1),
				Images = new List<AuctionImage>
				{
					new() { FilePath = "x.jpg" },
				},
				Bids = new List<Bid>
				{
					new() {
						Amount = 160m,
						Bidder =  new User {
							FirstName = "X",
							LastName = "X",
							Email = $"{Guid.NewGuid()}@mail.com",
							PasswordHash = "X",
							PasswordSalt = "X",
						}
					},
					new() {
						Amount = 170m,
						Bidder = new User {
							FirstName = "X",
							LastName = "X",
							Email = $"{Guid.NewGuid()}@mail.com",
							PasswordHash = "X",
							PasswordSalt = "X",
						}
					}
				},
				Seller = new User {
					FirstName = "X",
					LastName = "X",
					Email = $"{Guid.NewGuid()}@mail.com",
					PasswordHash = "X",
					PasswordSalt = "X",
				}
			};

			var endedAuction = new Auction {
				Name = "X",
				Description = "X",
				Status = (int)AuctionStatusEnum.Ended,
				BaselinePrice = 200m,
				StartTime = now,
				EndTime = now.AddHours(-3),
				Images = new List<AuctionImage>
				{
					new() { FilePath = "x.jpg" },
				},
				Bids = new List<Bid>
				{
					new() {
						Amount = 300m,
						Bidder = new User {
						FirstName = "X",
						LastName = "X",
						Email = $"{Guid.NewGuid()}@mail.com",
						PasswordHash = "X",
						PasswordSalt = "X",
						},
						IsWinningBid = true
					}
				},
				Seller = new User {
					FirstName = "X",
					LastName = "X",
					Email = $"{Guid.NewGuid()}@mail.com",
					PasswordHash = "X",
					PasswordSalt = "X",
				}
			};

			await _databaseContext.Auctions.AddRangeAsync(activeAuction, endedAuction);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new GetAllAuctionsQuery {
				PageNumber = 1,
				PageSize = 10,
				Filter = string.Empty,
				SortBy = nameof(Auction.Name),
				SortDesc = false,
				ActiveOnly = true
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var pagedResult = result.Value;

			// Verify pagination data
			Assert.Equal(1, pagedResult.PageNumber);
			Assert.Equal(10, pagedResult.PageSize);
			Assert.Equal(1, pagedResult.TotalRecords); // only 1 since -> ActiveOnly = true
			Assert.Single(pagedResult.Items);

			var DTO = pagedResult.Items.First();

			// Verify fields of the active auction
			Assert.Equal(activeAuction.Id, DTO.Id);
			Assert.Equal(activeAuction.Name, DTO.Name);
			Assert.Equal(activeAuction.Description, DTO.Description);
			Assert.Equal(activeAuction.Status, DTO.Status);
			Assert.Equal(activeAuction.BaselinePrice, DTO.BaselinePrice);
			Assert.Equal(activeAuction.StartTime, DTO.StartTime);
			Assert.Equal(activeAuction.EndTime, DTO.EndTime);
			Assert.Equal(2, DTO.NumberOfBids);
		}
	}
}

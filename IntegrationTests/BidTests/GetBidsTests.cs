using Application.Common.Tools.Time;
using Application.UseCases.Bidding.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.BidTests {
	public class GetBidsTests : BaseIntegrationTest {

		public GetBidsTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task GetBids_HappyPath() {

			// Arrange
			var now = TruncateTime.ToMinute(DateTime.UtcNow);

			var user = new User {
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X",
				PasswordSalt = "X",
				IsAdministrator = false,
				Bids = new List<Bid>() {
					new() {
						IsWinningBid = false,
						Amount = 100m,
						Auction = new Auction {
							Name = "X",
							Description = "X",
							BaselinePrice = 50m,
							StartTime = now,
							EndTime = now.AddHours(1),
							Status = (int)AuctionStatusEnum.Active,
							Seller = new User {
								FirstName = "X",
								LastName = "X",
								Email = $"{Guid.NewGuid()}@mail.com",
								PasswordHash = "X",
								PasswordSalt = "X",
							}
						}
					},
					new() {
						IsWinningBid = false,
						Amount = 50m,
						Auction = new Auction {
							Name = "X",
							Description = "X",
							BaselinePrice = 10m,
							StartTime = now,
							EndTime = now.AddDays(1),
							Status = (int)AuctionStatusEnum.Active,
							Seller = new User {
								FirstName = "X",
								LastName = "X",
								Email = $"{Guid.NewGuid()}@mail.com",
								PasswordHash = "X",
								PasswordSalt = "X",
							}
						}
					},
				}
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var query = new GetBidsQuery {
				UserId = user.Id,
				PageNumber = 1,
				PageSize = 10,
				SortBy = nameof(Bid.Amount),
				SortDesc = false
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var pagedResult = result.Value;

			// Verify pagination
			Assert.Equal(1, pagedResult.PageNumber);
			Assert.Equal(10, pagedResult.PageSize);
			Assert.Equal(2, pagedResult.TotalRecords);
			Assert.Equal(2, pagedResult.Items.Count);

			var dtoAmounts = pagedResult.Items.Select(x => x.Amount).ToList();
			Assert.Contains(50m, dtoAmounts);
			Assert.Contains(100m, dtoAmounts);
		}
	}
}
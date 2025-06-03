using Application.Common.Tools.Time;
using Application.UseCases.Profile.Queries;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;

namespace IntegrationTests.ProfileTests {
	public class GetProfileTests : BaseIntegrationTest {

		public GetProfileTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task GetProfile_HappyPath() {

			// Arrange

			// User with 2 auctions
			var now = TruncateTime.ToMinute(DateTime.UtcNow);
			var auction1Id = Guid.NewGuid();
			var auction2Id = Guid.NewGuid();

			var user = new User {
				FirstName = "X",
				LastName = "X",
				Email = $"{Guid.NewGuid()}@mail.com",
				PasswordHash = "X",
				PasswordSalt = "X",
				IsAdministrator = false,
				Wallet = new Wallet {
					Balance = 750m,
					FrozenBalance = 25m
				},
				Auctions = new List<Auction> {
					new() {
						Id = auction1Id,
						Name = "X",
						BaselinePrice = 100m,
						StartTime = now,
						EndTime = now.AddHours(3),
						Status = (int)AuctionStatusEnum.Active
					},
					new() {
						Id = auction2Id,
						Name = "",
						BaselinePrice = 200m,
						StartTime = now,
						EndTime = now.AddDays(1),
						Status = (int)AuctionStatusEnum.Paused
					}
				}
			};

			await _databaseContext.Users.AddAsync(user);
			await _databaseContext.SaveChangesAsync();

			var query = new GetProfileQuery {
				UserId = user.Id
			};

			// Act
			var result = await _mediator.Send(query, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var profileDTO = result.Value;

			// Verify user details
			Assert.Equal(user.Id, profileDTO.Id);
			Assert.Equal(user.Email, profileDTO.Email);

			// Verify wallet details
			Assert.Equal(user.Wallet.Id, profileDTO.WalletId);
			Assert.Equal(750m, profileDTO.Balance);
			Assert.Equal(25m, profileDTO.FrozenBalance);

			// Verify auctions details
			Assert.NotNull(profileDTO.OwnAuctions);
			Assert.Equal(2, profileDTO.OwnAuctions.Count);

			// Find and verify details of each auction
			var auction1DTO = profileDTO.OwnAuctions.FirstOrDefault(a => a.Id == auction1Id);
			var auction2DTO = profileDTO.OwnAuctions.FirstOrDefault(a => a.Id == auction2Id);

			Assert.NotNull(auction1DTO);
			Assert.Equal(100m, auction1DTO.BaselinePrice);
			Assert.Equal(now, auction1DTO.StartTime);
			Assert.Equal(now.AddHours(3), auction1DTO.EndTime);
			Assert.Equal((int)AuctionStatusEnum.Active, auction1DTO.Status);

			Assert.NotNull(auction2DTO);
			Assert.Equal(200m, auction2DTO.BaselinePrice);
			Assert.Equal(now, auction2DTO.StartTime);
			Assert.Equal(now.AddDays(1), auction2DTO.EndTime);
			Assert.Equal((int)AuctionStatusEnum.Paused, auction2DTO.Status);
		}
	}
}

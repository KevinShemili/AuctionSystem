using Application.UseCases.Auctions.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AuctionTests {
	public class DeleteAuctionTests : BaseIntegrationTest {
		public DeleteAuctionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task DeleteAuction_HappyPath() {

			// Arrange
			var auctionId = Guid.NewGuid();

			var user = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordHash = "X",
				PasswordSalt = "X",
				IsAdministrator = false,
				Auctions = new List<Auction>() {
					new() {
						Id = auctionId,
						Name = "X",
						Description = "X",
						BaselinePrice = 75m,
						StartTime = DateTime.UtcNow,
						EndTime = DateTime.UtcNow.AddHours(2),
						Status = (int)AuctionStatusEnum.Paused
					}
				}
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new DeleteAuctionCommand {
				UserId = user.Id,
				AuctionId = auctionId
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var deleted = await _databaseContext.Auctions.FirstOrDefaultAsync(a => a.Id == auctionId, CancellationToken.None);
			Assert.Null(deleted);
		}
	}
}
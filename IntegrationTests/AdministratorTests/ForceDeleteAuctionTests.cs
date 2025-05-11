using Application.UseCases.Administrator.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AdministratorTests {
	public class ForceDeleteAuctionTests : BaseIntegrationTest {

		public ForceDeleteAuctionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ForceDeleteAuction_HappyPath() {

			// Arrange
			var auctionId = Guid.NewGuid();

			var auction = new Auction {
				Id = auctionId,
				Status = (int)AuctionStatusEnum.Ended,
				Name = "X",
				Seller = new User {
					Id = Guid.NewGuid(),
					Email = $"{Guid.NewGuid()}@mail.com",
					FirstName = "X",
					LastName = "X",
					PasswordHash = "X",
					PasswordSalt = "X"
				},
				ForceClosedBy = Guid.NewGuid(),
			};

			_ = await _databaseContext.Auctions.AddAsync(auction);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new ForceDeleteAuctionCommand {
				AuctionId = auctionId
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var deletedAuction = await _databaseContext.Auctions.AsNoTracking()
																.IgnoreQueryFilters()
																.FirstOrDefaultAsync(a => a.Id == auctionId);
			Assert.NotNull(deletedAuction);
			Assert.True(deletedAuction.IsDeleted);
		}
	}
}

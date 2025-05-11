using Application.Common.ErrorMessages;
using Application.UseCases.Auctions.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AuctionTests {
	public class CreateAuctionTests : BaseIntegrationTest {

		public CreateAuctionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task CreateAuction_HappyPath() {

			// Arrange
			var user = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordHash = "X",
				PasswordSalt = "X"
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new CreateAuctionCommand {
				Name = "X",
				Description = "X",
				BaselinePrice = 150m,
				EndTime = DateTime.UtcNow.AddHours(2),
				Images = new List<string>() { "img" },
				SellerId = user.Id
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var auction = await _databaseContext.Auctions.Include(a => a.Images)
														 .FirstOrDefaultAsync(a => a.Id == result.Value);

			Assert.NotNull(auction);
			Assert.Equal(user.Id, auction.SellerId);
			Assert.Equal((int)AuctionStatusEnum.Active, auction.Status);
		}

		[Fact]
		public async Task CreateAuction_CalledByAdmin_Fails() {

			// Arrange
			var admin = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordHash = "X",
				PasswordSalt = "X",
				IsAdministrator = true
			};

			_ = await _databaseContext.Users.AddAsync(admin);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new CreateAuctionCommand {
				Name = "X",
				Description = "X",
				BaselinePrice = 150m,
				EndTime = DateTime.UtcNow.AddHours(2),
				Images = new List<string>() { "img" },
				SellerId = admin.Id
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.NotAccessibleByAdmins.Code, result.Error.Code);
		}
	}
}
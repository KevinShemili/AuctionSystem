using Application.UseCases.Auctions.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.AuctionTests {

	public class UpdateAuctionTests : BaseIntegrationTest {

		public UpdateAuctionTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task UpdateAuction_HappyPath() {

			// Arrange
			var uploadRoot = GetUploadRoot();

			var existingImagePath1 = Path.Combine(uploadRoot, "existing1.jpg");
			var existingImagePath2 = Path.Combine(uploadRoot, "existing2.jpg");
			await File.WriteAllTextAsync(existingImagePath1, "existing1");
			await File.WriteAllTextAsync(existingImagePath2, "existing2");

			var auctionId = Guid.NewGuid();

			var user = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				FirstName = "X",
				LastName = "X",
				PasswordHash = "X",
				PasswordSalt = "X",
				Auctions = new List<Auction>() {
					new() {
						Id = auctionId,
						Name = "X",
						Description = "X",
						BaselinePrice = 150m,
						StartTime = DateTime.UtcNow,
						EndTime = DateTime.UtcNow.AddHours(2),
						Status = (int)AuctionStatusEnum.Active,
						Images = new List<AuctionImage>() {
							new() {
								FilePath = "uploads/auctions/existing1.jpg",
								DateCreated = DateTime.UtcNow
							},
							new() {
								FilePath= "uploads/auctions/existing2.jpg",
								DateCreated = DateTime.UtcNow
							}
						}
					}
				}
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var newImage1 = "uploads/auctions/new1.jpg"; // Already added by controller
			var newImage2 = "uploads/auctions/new2.jpg"; // Already added by controller

			var command = new UpdateAuctionCommand {
				AuctionId = auctionId,
				UserId = user.Id,
				Name = "updated-name",
				Description = "updated-description",
				BaselinePrice = 2000m,
				EndTime = DateTime.UtcNow.AddHours(2),
				RemoveImages = new[] { "uploads/auctions/existing1.jpg", "uploads/auctions/existing2.jpg" },
				NewImages = new[] { newImage1, newImage2 }
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.False(File.Exists(existingImagePath1));
			Assert.False(File.Exists(existingImagePath2));

			var updatedAuction = await _databaseContext.Auctions.AsNoTracking()
																.Include(x => x.Images)
																.FirstOrDefaultAsync(x => x.Id == auctionId);

			Assert.Equal(2, updatedAuction.Images.Count);
			Assert.Equal("updated-name", updatedAuction.Name);
			Assert.Equal("updated-description", updatedAuction.Description);
			Assert.Equal(2000m, updatedAuction.BaselinePrice);
			Assert.Contains(newImage1, updatedAuction.Images.Select(x => x.FilePath));
			Assert.Contains(newImage2, updatedAuction.Images.Select(x => x.FilePath));
		}

		private string GetUploadRoot() {
			var environment = _serviceScope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

			string basePath = environment.WebRootPath ?? Directory.GetCurrentDirectory();
			string uploadRoot = Path.Combine(basePath, "uploads", "auctions");

			Directory.CreateDirectory(uploadRoot);

			return uploadRoot;
		}
	}
}

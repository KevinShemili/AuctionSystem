using Application.Common.ErrorMessages;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Application.UseCases.Auctions.Commands;
using Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.AuctionTests {

	public class CreateAuctionTests {

		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<ILogger<CreateAuctionCommandHandler>> _logger;
		private readonly CreateAuctionCommandHandler _handler;

		public CreateAuctionTests() {
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_auctionRepositoryMock = new Mock<IAuctionRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_logger = new Mock<ILogger<CreateAuctionCommandHandler>>();

			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true);

			_auctionRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Auction>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Auction auction, bool _, CancellationToken _) => auction);

			_handler = new CreateAuctionCommandHandler(
				_unitOfWorkMock.Object,
				_logger.Object,
				_auctionRepositoryMock.Object,
				_userRepositoryMock.Object
			);
		}

		[Fact]
		public async Task CreateAuction_UserNotFound_Fails() {

			// Arrange
			var command = new CreateAuctionCommand {
				SellerId = Guid.NewGuid(),
				BaselinePrice = 100m,
				EndTime = DateTime.UtcNow.AddMinutes(5),
				Images = new[] { "" }
			};

			_userRepositoryMock
				.Setup(x => x.GetByIdNoTrackingAsync(command.SellerId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((User)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.Unauthorized.Code, result.Error.Code);
		}

		[Fact]
		public async Task CreateAuction_UserIsAdministrator_Fails() {

			// Arrange
			var user = new User {
				Id = Guid.NewGuid(),
				IsAdministrator = true
			};

			_userRepositoryMock
				.Setup(x => x.GetByIdNoTrackingAsync(user.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			var command = new CreateAuctionCommand {
				SellerId = user.Id,
				BaselinePrice = 100m,
				EndTime = DateTime.UtcNow.AddMinutes(5),
				Images = new[] { "" }
			};

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.NotAccessibleByAdmins.Code, result.Error.Code);
		}

		[Theory]
		[InlineData(0)]
		[InlineData(-10)]
		public async Task CreateAuction_InvalidBaselinePrice_Fails(decimal price) {

			// Arrange
			var user = new User {
				Id = Guid.NewGuid(),
				IsAdministrator = false
			};

			_userRepositoryMock
				.Setup(x => x.GetByIdNoTrackingAsync(user.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			var command = new CreateAuctionCommand {
				SellerId = user.Id,
				BaselinePrice = price,
				EndTime = DateTime.UtcNow.AddMinutes(5),
				Images = new[] { "" }
			};

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.NegativeBaselinePrice.Code, result.Error.Code);
		}

		[Fact]
		public async Task CreateAuction_InvalidEndTime_Fails() {

			// Arrange
			var user = new User {
				Id = Guid.NewGuid(),
				IsAdministrator = false
			};

			_userRepositoryMock
				.Setup(x => x.GetByIdNoTrackingAsync(user.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			var command = new CreateAuctionCommand {
				SellerId = user.Id,
				BaselinePrice = 50m,
				EndTime = DateTime.UtcNow.AddMinutes(-5),
				Images = new[] { "i1" }
			};

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.EndSmallerEqualStart.Code, result.Error.Code);
		}

		[Fact]
		public async Task CreateAuction_NoImages_Fails() {

			// Arrange
			var user = new User {
				Id = Guid.NewGuid(),
				IsAdministrator = false
			};

			_userRepositoryMock
				.Setup(x => x.GetByIdNoTrackingAsync(user.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			var command = new CreateAuctionCommand {
				SellerId = user.Id,
				BaselinePrice = 50m,
				EndTime = DateTime.UtcNow.AddMinutes(5),
				Images = null
			};

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.OneOrMoreImages.Code, result.Error.Code);
		}

		[Fact]
		public async Task CreateAuction_HappyPath() {

			// Arrange
			var user = new User {
				Id = Guid.NewGuid(),
				IsAdministrator = false
			};

			_userRepositoryMock
				.Setup(x => x.GetByIdNoTrackingAsync(user.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			Auction captured = null!;
			_auctionRepositoryMock
				.Setup(x => x.CreateAsync(It.IsAny<Auction>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.Callback<Auction, bool, CancellationToken>((auction, _, _) => captured = auction)
				.ReturnsAsync((Auction auction, bool _, CancellationToken _) => auction);

			var command = new CreateAuctionCommand {
				SellerId = user.Id,
				Name = "X",
				Description = "X",
				BaselinePrice = 200m,
				EndTime = DateTime.UtcNow.AddMinutes(5),
				Images = new[] { "img" }
			};

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(captured.Id, result.Value);

			_auctionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Auction>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}

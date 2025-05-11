using Application.Common.Broadcast;
using Application.Common.ErrorMessages;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Application.UseCases.Bidding.Commands;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.BidTests {
	public class PlaceBidTests {

		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IBidRepository> _bidRepositoryMock;
		private readonly Mock<IBroadcastService> _broadcastServiceMock;
		private readonly Mock<IWalletTransactionRepository> _walletTransactionRepositoryMock;
		private readonly Mock<ILogger<PlaceBidCommandHandler>> _loggerMock;
		private readonly PlaceBidCommandHandler _handler;

		public PlaceBidTests() {
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_auctionRepositoryMock = new Mock<IAuctionRepository>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_bidRepositoryMock = new Mock<IBidRepository>();
			_broadcastServiceMock = new Mock<IBroadcastService>();
			_walletTransactionRepositoryMock = new Mock<IWalletTransactionRepository>();
			_loggerMock = new Mock<ILogger<PlaceBidCommandHandler>>();

			_unitOfWorkMock
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(true);

			_bidRepositoryMock
				.Setup(x => x.CreateAsync(It.IsAny<Bid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Bid bid, bool _, CancellationToken _) => bid);

			_bidRepositoryMock
				.Setup(x => x.UpdateAsync(It.IsAny<Bid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Bid b, bool _, CancellationToken _) => b);

			_walletTransactionRepositoryMock
				.Setup(x => x.CreateAsync(It.IsAny<WalletTransaction>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((WalletTransaction t, bool _, CancellationToken _) => t);

			_handler = new PlaceBidCommandHandler(
				_unitOfWorkMock.Object,
				_loggerMock.Object,
				_userRepositoryMock.Object,
				_auctionRepositoryMock.Object,
				_bidRepositoryMock.Object,
				_broadcastServiceMock.Object,
				_walletTransactionRepositoryMock.Object
			);
		}

		[Fact]
		public async Task PlaceBid_AuctionNotFound_Fails() {

			// Arrange
			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = Guid.NewGuid(),
				Amount = 100m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((Auction)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.AuctionNotFound(command.AuctionId).Code, result.Error.Code);
		}

		[Fact]
		public async Task PlaceBid_BidderIsSeller_Fails() {

			// Arrange
			var bidderId = Guid.NewGuid();

			var auction = new Auction {
				Id = Guid.NewGuid(),
				Seller = new User {
					Id = bidderId
				},
				Status = (int)AuctionStatusEnum.Active
			};

			var command = new PlaceBidCommand {
				AuctionId = auction.Id,
				BidderId = bidderId,
				Amount = 100m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(auction);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.BidderIsSeller.Code, result.Error.Code);
		}

		[Fact]
		public async Task PlaceBid_BidderNotFound_Fails() {

			// Arrange
			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = Guid.NewGuid(),
				Amount = 100m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User {
						Id = Guid.NewGuid()
					},
					Status = (int)AuctionStatusEnum.Active
				});

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(command.BidderId, It.IsAny<CancellationToken>()))
				.ReturnsAsync((User)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.Unauthorized.Code, result.Error.Code);
		}

		[Fact]
		public async Task PlaceBid_AdminPlacesBid_Fails() {

			// Arrange
			var bidder = new User {
				Id = Guid.NewGuid(),
				IsAdministrator = true
			};

			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = bidder.Id,
				Amount = 100m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User {
						Id = Guid.NewGuid()
					},
					Status = (int)AuctionStatusEnum.Active
				});

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(bidder.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(bidder);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.AdminsCannotPlaceBids.Code, result.Error.Code);
		}

		[Theory]
		[InlineData(2)]
		[InlineData(3)]
		public async Task PlaceBid_AuctionNotActive_Fails(int auctionStatus) {

			// Arrange
			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = Guid.NewGuid(),
				Amount = 100m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User { Id = Guid.NewGuid() },
					Status = auctionStatus
				});

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(command.BidderId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new User {
					Id = command.BidderId,
					IsAdministrator = false,
					Wallet = new Wallet {
						Balance = 1000m,
						FrozenBalance = 0m
					}
				});

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.AuctionNotActive.Code, result.Error.Code);
		}

		[Fact]
		public async Task PlaceBid_BidTooLow_Fails() {

			// Arrange
			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = Guid.NewGuid(),
				Amount = 100m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User { Id = Guid.NewGuid() },
					Status = (int)AuctionStatusEnum.Active,
					BaselinePrice = 200m // 200 > 100
				});

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(command.BidderId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new User {
					Id = command.BidderId,
					IsAdministrator = false,
					Wallet = new Wallet {
						Balance = 1000m,
						FrozenBalance = 0m
					}
				});

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.BidTooLow.Code, result.Error.Code);
		}

		[Fact]
		public async Task PlaceBid_ExistingPreviousBid_InsufficientFunds_Fails() {

			// Arrange
			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = Guid.NewGuid(),
				Amount = 260m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User { Id = Guid.NewGuid() },
					Status = (int)AuctionStatusEnum.Active,
					Bids = new List<Bid>() {
						new() {
							Id = Guid.NewGuid(),
							BidderId = command.BidderId,
							Amount = 200m
						}
					}
				});

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(command.BidderId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new User {
					Id = command.BidderId,
					IsAdministrator = false,
					Wallet = new Wallet {
						Balance = 250m,
						FrozenBalance = 200m
					}
				});

			// 260 - 200 = 60 ->  60 > 250 - 200 ? Yes

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.IncreaseBidInsufficientFunds.Code, result.Error.Code);
		}

		[Fact]
		public async Task PlaceBid_FirstTime_InsufficientFunds_Fails() {

			// Arrange
			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = Guid.NewGuid(),
				Amount = 200m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User { Id = Guid.NewGuid() },
					Status = (int)AuctionStatusEnum.Active,
					BaselinePrice = 50m,
					Bids = new List<Bid>()
				});

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(command.BidderId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new User {
					Id = command.BidderId,
					IsAdministrator = false,
					Wallet = new Wallet {
						Balance = 100m,
						FrozenBalance = 50m
					}
				});

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.InsufficientFunds.Code, result.Error.Code);
		}


		[Fact]
		public async Task PlaceBid_HappyPath_FirstTime() {

			// Arrange
			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = Guid.NewGuid(),
				Amount = 75m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User {
						Id = Guid.NewGuid()
					},
					Status = (int)AuctionStatusEnum.Active,
					BaselinePrice = 50m,
					Bids = new List<Bid>()
				});

			var bidder = new User {
				Id = command.BidderId,
				IsAdministrator = false,
				Wallet = new Wallet {
					Id = Guid.NewGuid(),
					Balance = 200m,
					FrozenBalance = 0m,
					Transactions = new List<WalletTransaction>()
				}
			};

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(command.BidderId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(bidder);

			Bid capturedBid = null!;
			_bidRepositoryMock
				.Setup(x => x.CreateAsync(It.IsAny<Bid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.Callback<Bid, bool, CancellationToken>((bid, _, _) => capturedBid = bid)
				.ReturnsAsync((Bid bid, bool _, CancellationToken _) => bid);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(capturedBid.Id, result.Value);

			_bidRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Bid>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
			_broadcastServiceMock.Verify(b => b.PublishAsync("NEW-BID", It.IsAny<object>()), Times.Once);
		}

		[Fact]
		public async Task PlaceBid_HappyPath_ExistingPreviousBid() {

			// Arrange
			var bidder = new User {
				Id = Guid.NewGuid(),
				IsAdministrator = false,
				Wallet = new Wallet {
					Id = Guid.NewGuid(),
					Balance = 300m,
					FrozenBalance = 50m
				}
			};

			var command = new PlaceBidCommand {
				AuctionId = Guid.NewGuid(),
				BidderId = bidder.Id,
				Amount = 150m
			};

			var existingBid = new Bid {
				Id = Guid.NewGuid(),
				BidderId = bidder.Id,
				Amount = 100m
			};

			_auctionRepositoryMock
				.Setup(x => x.GetAuctionWithBidsSellerWalletTransactionsAsync(command.AuctionId, It.IsAny<CancellationToken>()))
				.ReturnsAsync(new Auction {
					Id = command.AuctionId,
					Seller = new User { Id = Guid.NewGuid() },
					Status = (int)AuctionStatusEnum.Active,
					Bids = new List<Bid>() {
						existingBid
					}
				});

			_userRepositoryMock
				.Setup(x => x.GetUserWithWalletAndTransactionsAsync(bidder.Id, It.IsAny<CancellationToken>()))
				.ReturnsAsync(bidder);

			_walletTransactionRepositoryMock
				.Setup(x => x.CreateAsync(It.IsAny<WalletTransaction>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((WalletTransaction transaction, bool _, CancellationToken _) => transaction);

			_bidRepositoryMock
				.Setup(x => x.UpdateAsync(existingBid, It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync((Bid bid, bool _, CancellationToken _) => bid);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal(existingBid.Id, result.Value);

			_walletTransactionRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<WalletTransaction>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_bidRepositoryMock.Verify(x => x.UpdateAsync(existingBid, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}
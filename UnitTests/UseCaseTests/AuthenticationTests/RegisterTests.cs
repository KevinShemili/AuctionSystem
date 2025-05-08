using Application.Common.EmailService;
using Application.Common.ErrorMessages;
using Application.Common.TokenService;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Application.UseCases.Authentication.Commands;
using Domain.Entities;
using Domain.Enumerations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.UseCaseTests.AuthenticationTests {
	public class RegisterTests {

		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<ITokenService> _tokenServiceMock;
		private readonly Mock<IEmailService> _emailServiceMock;
		private readonly Mock<IConfiguration> _configMock;
		private readonly Mock<IUserTokenRepository> _userTokenRepositoryMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ILogger<RegisterCommandHandler>> _loggerMock;
		private readonly RegisterCommandHandler _handler;

		public RegisterTests() {
			_userRepositoryMock = new Mock<IUserRepository>();
			_userTokenRepositoryMock = new Mock<IUserTokenRepository>();
			_tokenServiceMock = new Mock<ITokenService>();
			_emailServiceMock = new Mock<IEmailService>();
			_configMock = new Mock<IConfiguration>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_loggerMock = new Mock<ILogger<RegisterCommandHandler>>();

			_handler = new RegisterCommandHandler(
				_userRepositoryMock.Object,
				_tokenServiceMock.Object,
				_emailServiceMock.Object,
				_configMock.Object,
				_userTokenRepositoryMock.Object,
				_unitOfWorkMock.Object,
				_loggerMock.Object
			);
		}

		[Fact]
		public async Task Register_EmailAlreadyExists_ReturnsFailure() {

			// Arrange
			var email = "test@mail.com";
			var command = new RegisterCommand { Email = email, Password = "Passw0rd123" };

			_userRepositoryMock.Setup(x => x.DoesEmailExistAsync(email, It.IsAny<CancellationToken>()))
							   .ReturnsAsync(true);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			_userRepositoryMock.Verify(x => x.DoesEmailExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
			Assert.False(result.IsSuccess);
			Assert.Equal(Errors.EmailAlreadyExists, result.Error);
		}

		[Fact]
		public async Task Register_HappyPath() {

			// Arrange
			var email = "test@mail.com";
			var password = "Passw0rd123";

			var userId = Guid.NewGuid();

			var command = new RegisterCommand { Email = email, Password = password };

			_userRepositoryMock.Setup(x => x.DoesEmailExistAsync(email, It.IsAny<CancellationToken>()))
							   .ReturnsAsync(false);

			var token = "random-token-test";
			_tokenServiceMock.Setup(x => x.GenerateEmailVerificationToken())
							 .Returns(token);

			var mappedUser = new User { Id = userId, Email = email };

			_configMock.Setup(x => x["VerificationTokenExpiries:ExpiryHours"])
					   .Returns("5");

			UserToken createdUserToken = null;
			_userTokenRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<UserToken>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
			  .Callback<UserToken, bool, CancellationToken>((userToken, flag, cancellationToken) => { createdUserToken = userToken; })
			  .ReturnsAsync((UserToken userToken, bool flag, CancellationToken cancellationToken) => userToken);

			User createdUser = null;
			_userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
							   .Callback<User, bool, CancellationToken>((user, flag, cancellationToken) => createdUser = user)
							   .ReturnsAsync((User user, bool flag, CancellationToken cancellationToken) => user);

			_emailServiceMock.Setup(e => e.SendConfirmationEmailAsync(token, email, It.IsAny<CancellationToken>()))
							 .Returns(Task.CompletedTask);

			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
						   .ReturnsAsync(true);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert

			_userRepositoryMock.Verify(x => x.DoesEmailExistAsync(email, It.IsAny<CancellationToken>()), Times.Once);
			_tokenServiceMock.Verify(x => x.GenerateEmailVerificationToken(), Times.Once);
			_userRepositoryMock.Verify(x => x.CreateAsync(mappedUser, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_userTokenRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserToken>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(token, email, It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

			Assert.NotNull(createdUserToken);
			Assert.Equal(token, createdUserToken.Token);
			Assert.Equal((int)TokenTypeEnum.EmailVerificationToken, createdUserToken.TokenTypeId);
			Assert.Equal(userId, createdUserToken.UserId);
			Assert.True(result.IsSuccess);
			Assert.True(result.Value);
		}
	}
}

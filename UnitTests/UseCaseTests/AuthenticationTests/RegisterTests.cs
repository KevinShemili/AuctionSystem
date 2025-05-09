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

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("xx")]
		[InlineData("lowercase")]
		[InlineData("UPPERCASE")]
		[InlineData("NONumbersss")]
		[InlineData("A1aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")]
		public async Task Register_InvalidPassword_ReturnsFailure(string badPwd) {

			// Arrange
			var cmd = new RegisterCommand {
				Email = "user@test.com",
				FirstName = "John",
				LastName = "Doe",
				Password = badPwd
			};

			// Act
			var result = await _handler.Handle(cmd, CancellationToken.None);

			// Assert
			_userRepositoryMock.Verify(x => x.DoesEmailExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
			Assert.False(result.IsSuccess);
			Assert.Equal(Errors.InvalidPasswordFormat, result.Error);
		}

		[Fact]
		public async Task Register_EmailAlreadyExists_ReturnsFailure() {

			// Arrange
			var command = new RegisterCommand { Email = "test@mail.com", Password = "Passw0rd123" };

			_userRepositoryMock.Setup(x => x.DoesEmailExistAsync(command.Email, It.IsAny<CancellationToken>()))
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

			var userId = Guid.NewGuid();

			var token = Guid.NewGuid().ToString();

			var command = new RegisterCommand {
				Email = "test@mail.com",
				Password = "Passw0rd123"
			};

			_userRepositoryMock.Setup(x => x.DoesEmailExistAsync(command.Email, It.IsAny<CancellationToken>()))
							   .ReturnsAsync(false);

			_tokenServiceMock.Setup(x => x.GenerateEmailVerificationToken())
						 .Returns(token);

			_configMock.Setup(x => x["VerificationTokenExpiries:ExpiryHours"])
				   .Returns("5");

			User capturedUser = null;
			UserToken capturedToken = null;
			_userRepositoryMock
				.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.Callback<User, bool, CancellationToken>((user, flag, cancellationToken) => capturedUser = user)
				.ReturnsAsync((User user, bool flag, CancellationToken cancellationToken) => user);

			_userTokenRepositoryMock
				.Setup(x => x.CreateAsync(It.IsAny<UserToken>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.Callback<UserToken, bool, CancellationToken>((userToken, flag, cancellationToken) => { capturedToken = userToken; })
				.ReturnsAsync((UserToken userToken, bool flag, CancellationToken cancellationToken) => userToken);

			_emailServiceMock.Setup(e => e.SendConfirmationEmailAsync(token, command.Email, It.IsAny<CancellationToken>()))
						 .Returns(Task.CompletedTask);

			_unitOfWorkMock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
					   .ReturnsAsync(true);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert

			_userRepositoryMock.Verify(x => x.DoesEmailExistAsync(command.Email, It.IsAny<CancellationToken>()), Times.Once);
			_tokenServiceMock.Verify(x => x.GenerateEmailVerificationToken(), Times.Once);
			_userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_userTokenRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<UserToken>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(token, command.Email, It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

			Assert.NotNull(capturedUser);
			Assert.NotNull(capturedToken);
			Assert.Equal(token, capturedToken.Token);
			Assert.Equal(command.Email, capturedUser.Email);
			Assert.Equal((int)TokenTypeEnum.EmailVerificationToken, capturedToken.TokenTypeId);
			Assert.True(result.IsSuccess);
			Assert.True(result.Value);
		}
	}
}

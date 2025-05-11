using Application.Common.ErrorMessages;
using Application.Common.TokenService;
using Application.Common.Tools.Passwords;
using Application.Common.Tools.Transcode;
using Application.Contracts.Repositories;
using Application.Contracts.Repositories.UnitOfWork;
using Application.UseCases.Authentication.Commands;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.AuthenticationTests {

	public class SignInTests {

		private readonly Mock<ITokenService> _tokenServiceMock;
		private readonly Mock<IConfiguration> _configurationMock;
		private readonly Mock<IUserRepository> _userRepositoryMock;
		private readonly Mock<IAuthenticationTokenRepository> _authTokenRepoMock;
		private readonly Mock<IUnitOfWork> _unitOfWorkMock;
		private readonly Mock<ILogger<SignInCommandHandler>> _loggerMock;
		private readonly SignInCommandHandler _handler;

		public SignInTests() {
			_tokenServiceMock = new Mock<ITokenService>();
			_configurationMock = new Mock<IConfiguration>();
			_userRepositoryMock = new Mock<IUserRepository>();
			_authTokenRepoMock = new Mock<IAuthenticationTokenRepository>();
			_unitOfWorkMock = new Mock<IUnitOfWork>();
			_loggerMock = new Mock<ILogger<SignInCommandHandler>>();

			_unitOfWorkMock
				.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
				.ReturnsAsync(true);

			_authTokenRepoMock
				.Setup(x => x.DeleteAsync(It.IsAny<AuthenticationToken>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
				.ReturnsAsync(true);

			_handler = new SignInCommandHandler(
				_tokenServiceMock.Object,
				_configurationMock.Object,
				_unitOfWorkMock.Object,
				_loggerMock.Object,
				_userRepositoryMock.Object,
				_authTokenRepoMock.Object
			);
		}

		[Fact]
		public async Task SignIn_UserNotFound_Fails() {

			// Arrange
			var command = new SignInCommand {
				Email = $"{Guid.NewGuid()}@mail.com",
				Password = "X"
			};

			_userRepositoryMock
				.Setup(x => x.GetUserWithAuthenticationTokensAsync(command.Email, It.IsAny<CancellationToken>()))
				.ReturnsAsync((User)null);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.UserNotFound("").Code, result.Error.Code);
		}

		[Fact]
		public async Task SignIn_EmailNotVerified_Fails() {

			// Arrange
			var user = new User {
				Email = $"{Guid.NewGuid()}@mail.com",
				IsEmailVerified = false
			};

			var command = new SignInCommand { Email = user.Email, Password = "X" };

			_userRepositoryMock
				.Setup(x => x.GetUserWithAuthenticationTokensAsync(user.Email, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.AccountNotVerified.Code, result.Error.Code);
		}

		[Fact]
		public async Task SignIn_UserBlocked_Fails() {

			// Arrange
			var user = new User {
				Email = $"{Guid.NewGuid()}@mail.com",
				IsEmailVerified = true,
				IsBlocked = true
			};

			var command = new SignInCommand { Email = user.Email, Password = "X" };

			_userRepositoryMock
				.Setup(x => x.GetUserWithAuthenticationTokensAsync(user.Email, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.LockedOut.Code, result.Error.Code);
		}

		[Fact]
		public async Task SignIn_PasswordIncorrect_Fails() {

			// Arrange
			var (hash, salt) = Hasher.HashPasword("Password");

			var user = new User {
				Email = $"{Guid.NewGuid()}@mail.com",
				IsEmailVerified = true,
				IsBlocked = false,
				FailedLoginTries = 1,
				PasswordHash = hash,
				PasswordSalt = salt,
				AuthenticationTokens = new List<AuthenticationToken>()
			};

			_configurationMock
				.Setup(x => x["FailedLogin:MaxTries"])
				.Returns("3");

			var command = new SignInCommand {
				Email = user.Email,
				Password = "X"
			};

			_userRepositoryMock
				.Setup(x => x.GetUserWithAuthenticationTokensAsync(user.Email, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.SignInFailure.Code, result.Error.Code);
			_userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(x => x.FailedLoginTries == 2), true, It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task SignIn_MaxPasswordIncorrectTries_Fails() {

			// Arrange
			var (hash, salt) = Hasher.HashPasword("Password");

			var user = new User {
				Email = $"{Guid.NewGuid()}@mail.com",
				IsEmailVerified = true,
				IsBlocked = false,
				FailedLoginTries = 3,
				PasswordHash = hash,
				PasswordSalt = salt,
				AuthenticationTokens = new List<AuthenticationToken>()
			};

			_configurationMock
				.Setup(x => x["FailedLogin:MaxTries"])
				.Returns("3");

			var command = new SignInCommand {
				Email = user.Email,
				Password = "X"
			};

			_userRepositoryMock
				.Setup(x => x.GetUserWithAuthenticationTokensAsync(user.Email, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.BlockReason("").Code, result.Error.Code);
			Assert.True(user.IsBlocked);
			_unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}

		[Fact]
		public async Task SignIn_HappyPath() {

			// Arrange
			var existingToken = new AuthenticationToken {
				Id = Guid.NewGuid(),
				RefreshToken = "X",
				AccessToken = "X",
			};

			var (hash, salt) = Hasher.HashPasword("Password");
			var user = new User {
				Id = Guid.NewGuid(),
				Email = $"{Guid.NewGuid()}@mail.com",
				IsEmailVerified = true,
				IsBlocked = false,
				FailedLoginTries = 0,
				PasswordHash = hash,
				PasswordSalt = salt,
				AuthenticationTokens = new List<AuthenticationToken> { existingToken }
			};

			_tokenServiceMock
				.Setup(t => t.GenerateAccessTokenAsync(user.Email, It.IsAny<CancellationToken>()))
				.ReturnsAsync("new-JWT");

			_tokenServiceMock
				.Setup(t => t.GenerateRefreshToken())
				.Returns(("new-refresh", DateTime.UtcNow.AddHours(2)));

			var command = new SignInCommand {
				Email = user.Email,
				Password = "Password"
			};

			_userRepositoryMock
				.Setup(x => x.GetUserWithAuthenticationTokensAsync(user.Email, It.IsAny<CancellationToken>()))
				.ReturnsAsync(user);

			// Act
			var result = await _handler.Handle(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.Equal("new-JWT", result.Value.AccessToken);
			Assert.Equal(Transcode.EncodeURL("new-refresh"), result.Value.RefreshToken);

			_authTokenRepoMock.Verify(r => r.DeleteAsync(existingToken, It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
			_unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
		}
	}
}

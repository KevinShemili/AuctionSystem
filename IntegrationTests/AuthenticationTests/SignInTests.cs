using Application.Common.ErrorMessages;
using Application.Common.Tools.Passwords;
using Application.UseCases.Authentication.Commands;
using Domain.Entities;
using IntegrationTests.Environment;

namespace IntegrationTests.AuthenticationTests {
	public class SignInTests : BaseIntegrationTest {
		public SignInTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task SignIn_HappyPath_ReturnsTokens() {

			// Arrange
			var email = $"{Guid.NewGuid()}@mail.com";
			var password = "x";

			(var passwordHash, var passwordSalt) = Hasher.HashPasword(password);

			_databaseContext.Users.Add(new User {
				Email = email,
				FirstName = "X",
				LastName = "X",
				PasswordSalt = passwordSalt,
				PasswordHash = passwordHash,
				IsEmailVerified = true
			});

			await _databaseContext.SaveChangesAsync();

			// Act
			var command = new SignInCommand { Email = email, Password = password };
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotNull(result.Value);
			Assert.NotNull(result.Value.RefreshToken);
			Assert.NotNull(result.Value.AccessToken);
		}

		[Fact]
		public async Task SignIn_UserDoesNotExist_Fails() {

			// Act
			var command = new SignInCommand { Email = $"{Guid.NewGuid}@mail.com", Password = "x" };
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.UserNotFound("").Code, result.Error.Code);

		}
	}
}

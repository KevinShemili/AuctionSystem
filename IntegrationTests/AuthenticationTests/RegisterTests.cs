using Application.Common.ErrorMessages;
using Application.UseCases.Authentication.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.AuthenticationTests {
	public class RegisterTests : BaseIntegrationTest {

		public RegisterTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task Register_HappyPath() {

			// Arrange
			var email = $"{Guid.NewGuid()}@mail.com";
			var command = new RegisterCommand {
				FirstName = "x",
				LastName = "x",
				Email = email,
				Password = "Passw0rd132"
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsSuccess);

			var user = await _databaseContext.Users.Include(x => x.Wallet)
												   .Include(x => x.VerificationTokens)
												   .FirstOrDefaultAsync(u => u.Email == email, CancellationToken.None);

			Assert.NotNull(user);
			Assert.Equal(email, user.Email);
			Assert.NotNull(user.Wallet);
			Assert.Equal(10000m, user.Wallet.Balance);
			Assert.NotNull(user.VerificationTokens);
			Assert.NotNull(user.VerificationTokens.FirstOrDefault(x => x.TokenTypeId == (int)VerificationTokenType.EmailVerificationToken));
		}

		[Fact]
		public async Task Register_EmailAlreadyExists_Fails() {

			// Arrange
			var email = $"{Guid.NewGuid()}@mail.com";
			_ = await _databaseContext.Users.AddAsync(new User {
				Email = email,
				FirstName = "X",
				LastName = "X",
				PasswordHash = "X",
				PasswordSalt = "X",
			});
			_ = await _databaseContext.SaveChangesAsync();

			var command = new RegisterCommand {
				FirstName = "x",
				LastName = "x",
				Email = email,
				Password = "Passw0rd132"
			};

			// Act
			var result = await _mediator.Send(command, CancellationToken.None);

			// Assert
			Assert.True(result.IsFailure);
			Assert.Equal(Errors.EmailAlreadyExists.Code, result.Error.Code);
		}
	}
}


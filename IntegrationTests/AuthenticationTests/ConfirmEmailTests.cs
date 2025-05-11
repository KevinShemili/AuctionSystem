using Application.Common.TokenService;
using Application.Common.Tools.Transcode;
using Application.UseCases.Authentication.Commands;
using Domain.Entities;
using Domain.Enumerations;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.AuthenticationTests {
	public class ConfirmEmailTests : BaseIntegrationTest {

		public ConfirmEmailTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task ConfirmEmail_HappyPath() {

			// Arrange
			var email = $"{Guid.NewGuid()}@example.com";

			var tokenService = _serviceScope.ServiceProvider.GetRequiredService<ITokenService>();
			var emailToken = tokenService.GenerateEmailVerificationToken();
			var encodedToken = Transcode.EncodeURL(emailToken);
			var expiry = DateTime.UtcNow.AddHours(1);

			var user = new User {
				Id = Guid.NewGuid(),
				Email = email,
				FirstName = "X",
				LastName = "X",
				PasswordHash = "X",
				PasswordSalt = "X",
				IsEmailVerified = false,
				VerificationTokens = new List<VerificationToken> {
					new() {
						Id = Guid.NewGuid(),
						Token = emailToken,
						Expiry = expiry,
						TokenTypeId = (int)VerificationTokenType.EmailVerificationToken
					}
				}
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new ConfirmEmailCommand {
				Email = email,
				Token = encodedToken
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);

			var updatedUser = await _databaseContext.Users.AsNoTracking()
														  .Include(x => x.VerificationTokens)
														  .FirstOrDefaultAsync(u => u.Id == user.Id);

			Assert.NotNull(updatedUser);
			Assert.True(updatedUser.IsEmailVerified);
		}
	}
}

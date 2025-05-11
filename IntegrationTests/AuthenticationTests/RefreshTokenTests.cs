using Application.Common.TokenService;
using Application.Common.Tools.Transcode;
using Application.UseCases.Authentication.Commands;
using Domain.Entities;
using IntegrationTests.Environment;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace IntegrationTests.AuthenticationTests {
	public class RefreshTokenTests : BaseIntegrationTest {

		public RefreshTokenTests(ContainerFactory<Program> factory) : base(factory) {
		}

		[Fact]
		public async Task RefreshToken_HappyPath() {

			// Arrange			
			var email = $"{Guid.NewGuid()}@mail.com";
			var userId = Guid.NewGuid();

			var tokenService = _serviceScope.ServiceProvider.GetRequiredService<ITokenService>();

			var claims = new[] { new Claim(ClaimTypes.Email, email) };
			var initialAccessToken = tokenService.GenerateAccessToken(claims);

			var (initialRawRefreshToken, initialExpiry) = tokenService.GenerateRefreshToken();

			var user = new User {
				Id = userId,
				Email = email,
				FirstName = "X",
				LastName = "X",
				AuthenticationTokens = new List<AuthenticationToken>() {
					new () {
						Id = Guid.NewGuid(),
						AccessToken = initialAccessToken,
						RefreshToken = initialRawRefreshToken,
						Expiry = initialExpiry
					}
				}
			};

			_ = await _databaseContext.Users.AddAsync(user);
			_ = await _databaseContext.SaveChangesAsync();

			var command = new RefreshTokenCommand {
				AccessToken = initialAccessToken,
				RefreshToken = Transcode.EncodeURL(initialRawRefreshToken)
			};

			// Act
			var result = await _mediator.Send(command);

			// Assert
			Assert.True(result.IsSuccess);
			Assert.NotNull(result.Value);
			Assert.False(string.IsNullOrEmpty(result.Value.AccessToken));
			Assert.False(string.IsNullOrEmpty(result.Value.RefreshToken));

			var updatedUser = await _databaseContext.Users.AsNoTracking()
														  .Include(x => x.AuthenticationTokens)
														  .FirstOrDefaultAsync(x => x.Id == userId);

			Assert.Single(updatedUser.AuthenticationTokens);

			var newAccessTokenClaims = tokenService.GetClaims(result.Value.AccessToken);
			var newAccessTokenEmail = newAccessTokenClaims.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Email).Value;
			Assert.Equal(email, newAccessTokenEmail);
		}
	}
}
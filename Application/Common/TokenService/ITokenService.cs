using System.Security.Claims;

namespace Application.Common.TokenService {
	public interface ITokenService {
		Task<string> GenerateAccessTokenAsync(string email, CancellationToken cancellationToken);
		(string, DateTime) GenerateRefreshToken();
		string GenerateEmailVerificationToken();
		ClaimsPrincipal GetClaims(string accessToken);
		string GenerateAccessToken(IEnumerable<Claim> claims);
	}
}

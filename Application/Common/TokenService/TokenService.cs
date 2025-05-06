using Application.Contracts.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Common.TokenService {
	public class TokenService : ITokenService {

		private readonly IConfiguration _configuration;
		private readonly IUserRepository _userRepository;

		public TokenService(IConfiguration configuration, IUserRepository userRepository) {
			_configuration = configuration;
			_userRepository = userRepository;
		}

		public async Task<string> GenerateAccessTokenAsync(string email, CancellationToken cancellationToken) {

			var user = await _userRepository.GetUserWithRolesAndPermissionsNoTrackingAsync(email, cancellationToken: cancellationToken);

			if (user is null)
				ArgumentNullException.ThrowIfNull(user);

			var tokenClaims = new List<Claim> {
							new (ClaimTypes.Email, user.Email),
							new (ClaimTypes.Name, user.LastName),
							new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
							new (JwtRegisteredClaimNames.Sub, user.Id.ToString())};

			if (user.Roles.Count != 0)
				foreach (var role in user.Roles)
					if (role.Permissions.Count != 0)
						foreach (var permission in role.Permissions)
							tokenClaims.Add(new Claim("Permissions", permission.Key));

			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_configuration["JWTSettings:Secret"]!));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var tokenExpiry = DateTime.UtcNow.AddHours(
					Convert.ToDouble(_configuration["JWTSettings:ExpiryHour"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWTSettings:Issuer"],
				audience: _configuration["JWTSettings:Audience"],
				claims: tokenClaims,
				expires: tokenExpiry,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public Guid GetUserId(string accessToken) {
			var token = new JwtSecurityToken(accessToken);
			var userId = Guid.Parse(token.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);
			return userId;
		}

		public string GenerateAccessToken(IEnumerable<Claim> claims) {
			var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(_configuration["JWTSettings:Secret"]!));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var tokenExpiry = DateTime.UtcNow.AddHours(
					Convert.ToDouble(_configuration["JWTSettings:ExpiryHour"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["JWTSettings:Issuer"],
				audience: _configuration["JWTSettings:Audience"],
				claims: claims,
				expires: tokenExpiry,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		public (string, DateTime) GenerateRefreshToken() {
			return (
				Guid.NewGuid().ToString(),
				DateTime.UtcNow.AddHours(
					Convert.ToDouble(_configuration["RefreshTokenSettings:ExpiryDays"]))
				);
		}

		public string GenerateEmailVerificationToken() {
			return Guid.NewGuid().ToString();
		}

		public ClaimsPrincipal GetClaims(string accessToken) {
			var validationParameters = new TokenValidationParameters {
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = _configuration["JWTSettings:Issuer"],
				ValidAudience = _configuration["JWTSettings:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWTSettings:Secret"]!)),
				ClockSkew = TimeSpan.Zero,
				RequireExpirationTime = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(accessToken, validationParameters, out SecurityToken securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
				throw new SecurityTokenException("Invalid token");

			return principal;
		}
	}
}

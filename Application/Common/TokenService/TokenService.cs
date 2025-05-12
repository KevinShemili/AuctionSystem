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

		// Given a user email, get all of his roles and permissions
		// and generate an appropriate JWT token.
		// The resulting token will contain:
		// 1. User ID
		// 2. User permissions
		// 3. Token ID
		// 4. Token expiration date
		public async Task<string> GenerateAccessTokenAsync(string email, CancellationToken cancellationToken) {

			var user = await _userRepository.GetUserWithRolesAndPermissionsNoTrackingAsync(email, cancellationToken: cancellationToken);

			if (user is null)
				ArgumentNullException.ThrowIfNull(user);

			var tokenClaims = new List<Claim> {
							new (ClaimTypes.NameIdentifier, user.Id.ToString()), // What controller reads
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

		// Given a predefined list of claims
		// use it to generate a JWT token.
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

		// Generate a refresh token and its expiration date.
		// The refresh token is simply a random string in the form of a GUID.
		public (string, DateTime) GenerateRefreshToken() {
			return (
				Guid.NewGuid().ToString(),
				DateTime.UtcNow.AddHours(
					Convert.ToDouble(_configuration["RefreshTokenSettings:ExpiryDays"]))
				);
		}

		// Generate a random email verification token.
		public string GenerateEmailVerificationToken() {
			return Guid.NewGuid().ToString();
		}

		// Given a JWT token we use this method to extract its claims.
		// Claims are JWT attributes that are used to store information.
		public ClaimsPrincipal GetClaims(string accessToken) {

			// We make sure to validate the token before extracting its claims.
			// Thus ensuring the token is valid and not expired.
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

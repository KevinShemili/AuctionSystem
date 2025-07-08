using Application.Common.ResultPattern;
using Application.UseCases.Authentication.Commands;
using Application.UseCases.Authentication.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Controllers.Common;
using WebAPI.DTOs;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("api/auth")]
	public class AuthenticationController : AbstractController {

		public AuthenticationController(IMediator mediator) : base(mediator) {
		}

		[SwaggerOperation(
			Summary = "Register new account",
			Description = @"
			Creates a new user account. Validates password format, checks for existing email, generates an email verification token, 
			initializes the user’s wallet with a default balance, 
			and sends a confirmation email

			Request body:
			- firstName (string, required): User’s first name
			- lastName (string, required): User’s last name
			- email (string, required): User’s email address (must be unique)
			- password (string, required): Password (8–50 characters, at least one digit, one uppercase letter, one lowercase letter, no special characters)")]
		[AllowAnonymous]
		[HttpPost("register")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO) {

			var command = new RegisterCommand {
				Email = registerDTO.Email,
				FirstName = registerDTO.FirstName,
				LastName = registerDTO.LastName,
				Password = registerDTO.Password
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Sign In",
			Description = @"
			Authenticates a user using email and password. Validates credentials,
			checks email verification and block status, tracks failed login attempts (blocking on too many failures), 
			and returns a JWT access token & refresh token on success

			Request body:
			- email (string, required): User's registered email
			- password (string, required): User's password")]
		[AllowAnonymous]
		[HttpPost("login")]
		[ProducesResponseType(typeof(SignInDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status429TooManyRequests)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> SignIn([FromBody] SignInRequestDTO signInDTO) {

			var command = new SignInCommand {
				Email = signInDTO.Email,
				Password = signInDTO.Password
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Refresh Token & JWT",
			Description = @"
			Validates the provided access token and refresh token, then issues a new access token and refresh token pair

			Request body:
			- accessToken (string, required): Current JWT access token
			- refreshToken (string, required): Current refresh token")]
		[AllowAnonymous]
		[HttpPost("refresh-token")]
		[ProducesResponseType(typeof(RefreshTokenDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> Refresh([FromBody] TokensDTO tokensDTO) {

			var command = new RefreshTokenCommand {
				AccessToken = tokensDTO.AccessToken,
				RefreshToken = tokensDTO.RefreshToken
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Confirm Email",
			Description = @"
			Validates the provided email verification token and marks the user's email as verified
			If the token has expired, a new one is issued and sent

			Request body:
			- email (string, required): User’s email address
			- token (string, required): Encoded email verification token")]
		// [FromQuery] is always a GET request.
		[AllowAnonymous]
		[HttpGet("confirm-email")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email) {

			var command = new ConfirmEmailCommand {
				Email = email,
				Token = token
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}
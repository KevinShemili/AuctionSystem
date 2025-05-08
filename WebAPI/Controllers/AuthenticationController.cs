using Application.UseCases.Authentication.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Controllers.Common;
using WebAPI.DTOs.AuthenticationDTOs;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("api/[controller]")]
	public class AuthenticationController : AbstractController {

		public AuthenticationController(IMediator mediator) : base(mediator) {
		}

		[AllowAnonymous]
		[SwaggerOperation(Summary = "Register Account")]
		[HttpPost("register")]
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

		[AllowAnonymous]
		[SwaggerOperation(Summary = "Sign In")]
		[HttpPost("sign-in")]
		public async Task<IActionResult> SignIn([FromBody] SignInDTO signInDTO) {

			var command = new SignInCommand {
				Email = signInDTO.Email,
				Password = signInDTO.Password
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[AllowAnonymous]
		[SwaggerOperation(Summary = "Refresh Token")]
		[HttpPost("refresh")]
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

		// [FromQuery] is always a GET request.
		[AllowAnonymous]
		[SwaggerOperation(Summary = "Confirm Email")]
		[HttpGet("confirm-email")]
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

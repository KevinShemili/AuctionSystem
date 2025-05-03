using Application.UseCases.Authentication.Commands;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Controllers.Common;
using WebAPI.DTOs.AuthenticationDTOs;

namespace WebAPI.Controllers {

	public class AuthenticationController : AbstractController {

		public AuthenticationController(IMediator mediator, IMapper mapper) : base(mediator, mapper) {
		}

		[AllowAnonymous]
		[SwaggerOperation(Summary = "Register Account")]
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO) {

			var command = _mapper.Map<RegisterCommand>(registerDTO);

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[AllowAnonymous]
		[HttpGet("throw")]
		public IActionResult Throw() {
			throw new Exception("This is a test exception.");
		}
	}
}

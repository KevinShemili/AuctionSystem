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

		private readonly ILogger<AuthenticationController> _logger;

		public AuthenticationController(IMediator mediator,
								  IMapper mapper,
								  ILogger<AuthenticationController> logger) : base(mediator, mapper) {
			_logger = logger;
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

		[AllowAnonymous]
		[HttpGet("log")]
		public IActionResult LogTest() {

			_logger.LogInformation("LogTest test: {Now}", DateTime.UtcNow);


			return Ok("Log Test Done.");
		}
	}
}

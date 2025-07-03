using Application.Common.ResultPattern;
using Application.UseCases.Profile.DTOs;
using Application.UseCases.Profile.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Controllers.Common;

namespace WebAPI.Controllers {
	[ApiController]
	[Route("api/profile")]
	public class UserController : AbstractController {
		public UserController(IMediator mediator) : base(mediator) {
		}

		[SwaggerOperation(
			Summary = "Profile of current user",
			Description = @"
			Retrieves the profile for the user making the request, 
			including wallet details (balance, frozen balance),
			a list of their created auctions,
			and a list of their placed bids if a normal user.
			Otherwise shows roles / permissions of admin making the request.")]
		[Authorize]
		[HttpGet]
		[ProducesResponseType(typeof(ProfileDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> GetMe() {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var query = new GetProfileQuery {
				UserId = userId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure) {
				return StatusCode(result.Error.Code, result.Error.Message);
			}

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Current user wallet & transactions",
			Description = @"
			Retrieves the wallet details of the user making the request, 
			including balance, frozen balance, and transaction history.")]
		[Authorize]
		[HttpGet("wallet")]
		[ProducesResponseType(typeof(WalletDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ViewTransactions() {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var query = new ViewMyWalletQuery {
				UserId = userId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}
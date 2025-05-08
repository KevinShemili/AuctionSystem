using Application.UseCases.Profile.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebAPI.Controllers.Common;

namespace WebAPI.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class UserController : AbstractController {
		public UserController(IMediator mediator) : base(mediator) {
		}

		[Authorize]
		[HttpGet("me")]
		public async Task<IActionResult> GetMe() {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var query = new GetProfileQuery {
				UserId = userId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure) {
				return StatusCode(result.Error.Code, result.Error.Message);
			}

			var response = result.Value;

			var baseUrl = $"{Request.Scheme}://{Request.Host}/";

			foreach (var auction in response.CreatedAuctions) {
				foreach (var image in auction.Images) {
					image.Url = baseUrl + image.Url;
				}
			}

			foreach (var auction in response.ParticipatedAuctions) {
				foreach (var image in auction.Images) {
					image.Url = baseUrl + image.Url;
				}
			}

			return Ok(response);
		}
	}
}

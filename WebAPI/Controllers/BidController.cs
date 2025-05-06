using Application.UseCases.Bidding.Commands;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Controllers.Common;
using WebAPI.DTOs.BidDTOs;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("api/[controller]")]
	public class BidController : AbstractController {

		public BidController(IMediator mediator, IMapper mapper) : base(mediator, mapper) {
		}

		[Authorize]
		[SwaggerOperation(Summary = "Place Bid")]
		[HttpPost]
		public async Task<IActionResult> PlaceBid([FromBody] PlaceBidDTO dto) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var command = _mapper.Map<PlaceBidCommand>(dto);
			command.BidderId = userId;

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}

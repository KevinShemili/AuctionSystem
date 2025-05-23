﻿using Application.UseCases.Bidding.Commands;
using Application.UseCases.Bidding.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Controllers.Common;
using WebAPI.DTOs;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("api/bids")]
	public class BidController : AbstractController {

		public BidController(IMediator mediator) : base(mediator) {
		}

		[Authorize]
		[SwaggerOperation(Summary = "Place a bid")]
		[HttpPost]
		public async Task<IActionResult> PlaceBid([FromBody] PlaceBidDTO dto) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var command = new PlaceBidCommand {
				BidderId = userId,
				Amount = dto.Amount,
				AuctionId = dto.AuctionId
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize]
		[SwaggerOperation(Summary = "Current user bid history")]
		[HttpGet]
		public async Task<IActionResult> ViewBids([FromQuery] PagedParamsDTO pagedParams) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var query = new GetBidsQuery {
				UserId = userId,
				Filter = pagedParams.Filter,
				PageNumber = pagedParams.PageNumber,
				PageSize = pagedParams.PageSize,
				SortBy = pagedParams.SortBy,
				SortDesc = pagedParams.SortDesc
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}

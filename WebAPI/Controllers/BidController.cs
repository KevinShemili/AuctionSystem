using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.UseCases.Bidding.Commands;
using Application.UseCases.Bidding.DTOs;
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

		[SwaggerOperation(
			Summary = "Place a bid",
			Description = @"
			Places a bid for a user on a specific auction.
			- Auction must exist and be active.
			- Bidder cannot be the seller or an administrator.
			- Bid amount must be at least the baseline price.
			- User must have sufficient available balance.
			- If the bidder has already placed a bid, the existing bid is updated (with funds adjusted).
			- Broadcasts a NEW-BID event to notify clients.

			Request body:
			- auctionId (GUID, required): ID of the auction to place a bid on.
			- bidderId (GUID, required): ID of the user placing the bid.
			- amount (decimal, required): Amount of the bid.")]
		[Authorize]
		[HttpPost]
		[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Current user bid history",
			Description = @"
			Retrieves a paginated list of bids placed by the user making the request.

			Query parameters:
			- pageNumber (int, optional): Page index. Defaults to 1.  
			- pageSize (int, optional): Number of items per page. Defaults to 10.
			- sortBy (string, optional): Field name to sort on.
			- sortDesc (bool, optional): true -> descending. false -> ascending")]
		[Authorize]
		[HttpGet]
		[ProducesResponseType(typeof(PagedResponse<BidDTO>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ViewBids([FromQuery] PagedParamsNoFilterDTO pagedParams) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var query = new GetBidsQuery {
				UserId = userId,
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
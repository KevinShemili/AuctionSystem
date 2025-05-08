using Application.UseCases.Auctions.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Controllers.Common;
using WebAPI.DTOs.AuctionDTOs;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("api/[controller]")]
	public class AuctionController : AbstractController {

		public AuctionController(IMediator mediator) : base(mediator) {
		}

		[Authorize]
		[SwaggerOperation(Summary = "Create Auction")]
		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateAuction([FromForm] CreateAuctionDTO dto, [FromForm] ImagesDTO images) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var imageBytes = new List<byte[]>();
			foreach (var file in images.Images) {
				using var ms = new MemoryStream();
				await file.CopyToAsync(ms);
				imageBytes.Add(ms.ToArray());
			}

			var command = new CreateAuctionCommand {
				StartTime = dto.StartTime,
				EndTime = dto.EndTime,
				BaselinePrice = dto.BaselinePrice,
				Description = dto.Description,
				Name = dto.Name,
				SellerId = userId,
				Images = imageBytes
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}

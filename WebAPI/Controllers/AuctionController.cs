using Application.UseCases.Auctions.Commands;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WebAPI.Controllers.Common;
using WebAPI.DTOs.AuctionDTOs;

namespace WebAPI.Controllers {
	public class AuctionController : AbstractController {
		public AuctionController(IMediator mediator, IMapper mapper) : base(mediator, mapper) {
		}

		[AllowAnonymous]
		[SwaggerOperation(Summary = "Create Auction")]
		[HttpPost()]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateAuction([FromHeader] string accessToken, [FromForm] CreateAuctionDTO dto, [FromForm] ImagesDTO images) {

			var imageBytes = new List<byte[]>();
			foreach (var file in images.Images) {
				using var ms = new MemoryStream();
				await file.CopyToAsync(ms);
				imageBytes.Add(ms.ToArray());
			}

			var command = _mapper.Map<CreateAuctionCommand>(dto);
			command.AccessToken = accessToken;
			command.Images = imageBytes;

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}

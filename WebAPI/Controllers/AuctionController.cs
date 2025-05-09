using Application.UseCases.Auctions.Commands;
using Application.UseCases.Auctions.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Controllers.Common;
using WebAPI.DTOs;

namespace WebAPI.Controllers {

	[ApiController]
	[Route("api")]
	public class AuctionController : AbstractController {

		private readonly IWebHostEnvironment _webHostEnvironment;

		public AuctionController(IMediator mediator, IWebHostEnvironment webHostEnvironment) : base(mediator) {
			_webHostEnvironment = webHostEnvironment;
		}

		[Authorize]
		[SwaggerOperation(Summary = "Create an auction")]
		[HttpPost("auctions")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateAuction([FromForm] CreateAuctionDTO createAuctionDTO) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			(var error, var imageDirectories) = await StoreInDisk(createAuctionDTO.Images);

			if (error != null)
				return error;

			var command = new CreateAuctionCommand {
				StartTime = createAuctionDTO.StartTime,
				EndTime = createAuctionDTO.EndTime,
				BaselinePrice = createAuctionDTO.BaselinePrice,
				Description = createAuctionDTO.Description,
				Name = createAuctionDTO.Name,
				SellerId = userId,
				Images = imageDirectories
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize]
		[SwaggerOperation(Summary = "Update an auction")]
		[HttpPut("auctions/{auctionId}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateAuction([FromRoute] Guid auctionId, [FromForm] UpdateAuctionDTO dto) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var imageDirectories = new List<string>();
			if (dto.Images != null) {

				(var error, var path) = await StoreInDisk(dto.Images);

				if (error != null)
					return error;

				imageDirectories = path;
			}

			var cmd = new UpdateAuctionCommand {
				AuctionId = auctionId,
				UserId = userId,
				Name = dto.Name,
				Description = dto.Description,
				BaselinePrice = dto.BaselinePrice,
				StartTime = dto.StartTime,
				EndTime = dto.EndTime,
				NewImages = imageDirectories
			};

			var result = await _mediator.Send(cmd);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		private async Task<(IActionResult ErrorResult, List<string> ImagePaths)> StoreInDisk(IEnumerable<IFormFile> images) {

			var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "auctions");
			var imageDirectories = new List<string>();
			var allowedExts = new[] { ".jpg", ".jpeg", ".png" };


			foreach (var image in images) {

				var fileExtension = Path.GetExtension(image.FileName).ToLower();
				if (allowedExts.Contains(fileExtension) is false ||
					image.ContentType.StartsWith("image/") is false) {
					return (BadRequest($"Only allowed: {string.Join(", ", allowedExts)}"), null);
				}

				var fileName = $"{Guid.NewGuid()}{fileExtension}";
				var fullPath = Path.Combine(uploadDirectory, fileName);

				await using var fs = System.IO.File.Create(fullPath);
				await image.CopyToAsync(fs);

				imageDirectories.Add(Path.Combine("uploads", "auctions", fileName).Replace("\\", "/"));
			}

			return (null, imageDirectories);
		}

		[AllowAnonymous]
		[SwaggerOperation(Summary = "See all auctions (Ended (O) + Active)")]
		[HttpGet("auctions")]
		public async Task<IActionResult> ViewAuctions([FromQuery] PagedParamsDTO pagedParams, [FromQuery] bool activeOnly) {

			var query = new GetAllAuctionsQuery {
				ActiveOnly = activeOnly,
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

		[AllowAnonymous]
		[SwaggerOperation(Summary = "See auction details")]
		[HttpGet("auctions/{auctionId}")]
		public async Task<IActionResult> ViewAuction([FromRoute] Guid auctionId) {

			var query = new GetAuctionQuery {
				AuctionId = auctionId,
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize]
		[SwaggerOperation(Summary = "Put auction in status pause")]
		[HttpPatch("auctions/{auctionId}/pause")]
		public async Task<IActionResult> PauseAuction([FromRoute] Guid auctionId) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var command = new PauseAuctionCommand {
				UserId = userId,
				AuctionId = auctionId
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize]
		[SwaggerOperation(Summary = "Put auction in status active")]
		[HttpPatch("auctions/{auctionId}/resume")]
		public async Task<IActionResult> ResumeAuction([FromRoute] Guid auctionId, [FromBody] DateTime endTime) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var command = new ResumeAuctionCommand {
				UserId = userId,
				AuctionId = auctionId,
				EndTime = endTime
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize]
		[SwaggerOperation(Summary = "Delete auction")]
		[HttpDelete("auctions/{auctionId}/resume")]
		public async Task<IActionResult> DeleteAuction([FromRoute] Guid auctionId) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var query = new DeleteAuctionCommand {
				UserId = userId,
				AuctionId = auctionId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}

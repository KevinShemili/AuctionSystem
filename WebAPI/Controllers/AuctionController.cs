using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.UseCases.Auctions.Commands;
using Application.UseCases.Auctions.DTOs;
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

		[SwaggerOperation(
			Summary = "Create an auction",
			Description = @"
			Creates a new auction with the provided details. Validates that the seller is not an administrator, 
			the baseline price is greater than zero, 
			the end time is in the future (normalized to minute precision), 
			and at least one image is supplied.

			Request body:
			- name (string, required): Auction title.
			- description (string, optional): Auction description.
			- baselinePrice (decimal, required): Starting price (must be greater than 0).
			- endTime (DateTime, required): Auction end time (must be after the start time, normalized to minute).
			- images (IEnumerable<IFormFile>, required): List of image file paths or URLs (at least one non-empty string).
			- sellerId (GUID, required): ID of the user creating the auction (must not be an administrator).")]
		[Authorize]
		[HttpPost("auctions")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> CreateAuction([FromForm] CreateAuctionDTO createAuctionDTO) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			(var error, var paths) = await StoreInDisk(createAuctionDTO.Images);

			if (error != null)
				return error;

			var command = new CreateAuctionCommand {
				EndTime = createAuctionDTO.EndTime,
				BaselinePrice = createAuctionDTO.BaselinePrice,
				Description = createAuctionDTO.Description,
				Name = createAuctionDTO.Name,
				SellerId = userId,
				Images = paths
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Update an auction",
			Description = @"
			Updates auction details such as name, description, baseline price, end time, and images.
			- Only the seller (userId) can perform the update.
			- Auction cannot have active bids.
			- If endTime is provided, it must be in the future.
			- Removing images must leave at least one image.

			Request body:
			- auctionId (GUID, required): ID of the auction to update.
			- userId (GUID, required): ID of the user performing the update.
			- name (string, optional): New title for the auction.
			- description (string, optional): New description for the auction.
			- baselinePrice (decimal, optional): New baseline price.
			- endTime (DateTime, optional): New end time for the auction.
			- newImages (IEnumerable<FormFile>, optional): List of new image file paths to add.
			- removeImages (IEnumerable<string>, optional): List of existing image file paths to remove.")]
		[Authorize]
		[HttpPut("auctions/{auctionId}")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> UpdateAuction([FromRoute] Guid auctionId, [FromForm] UpdateAuctionDTO updateAuctionDTO) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var imageDirectories = new List<string>();

			if (updateAuctionDTO.NewImages != null && updateAuctionDTO.NewImages.Any()) {

				(var error, var paths) = await StoreInDisk(updateAuctionDTO.NewImages);

				if (error != null)
					return error;

				imageDirectories.AddRange(paths);
			}

			var cmd = new UpdateAuctionCommand {
				AuctionId = auctionId,
				UserId = userId,
				Name = updateAuctionDTO.Name,
				Description = updateAuctionDTO.Description,
				BaselinePrice = updateAuctionDTO.BaselinePrice,
				EndTime = updateAuctionDTO.EndTime,
				NewImages = imageDirectories,
				RemoveImages = updateAuctionDTO.RemoveImages
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

		[SwaggerOperation(
			Summary = "See all auctions (Ended (O) + Active)",
			Description = @"
			Retrieves a paged list of auctions.

			Query parameters:
			- pageNumber (int, optional): Page index. Defaults to 1.  
			- pageSize (int, optional): Number of items per page. Defaults to 10.
			- filter (string, optional): Match against filter.
			- sortBy (string, optional): Field name to sort on.
			- sortDesc (bool, optional): true -> descending. false -> ascending
			- activeOnly(bool, required): true to return only active auctions, false to return all auctions.")]
		[AllowAnonymous]
		[HttpGet("auctions")]
		[ProducesResponseType(typeof(PagedResponse<AuctionDTO>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "See auction details",
			Description = @"
			Retrieves detailed information for a specific auction, including seller information, 
			images, and a list of bidders (names only -> bid amounts are not exposed).

			Path parameters:
			- auctionId (GUID, required): Unique identifier of the auction to retrieve.")]
		[Authorize]
		[HttpGet("auctions/{auctionId}")]
		[ProducesResponseType(typeof(AuctionDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ViewAuction([FromRoute] Guid auctionId) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var query = new GetAuctionQuery {
				AuctionId = auctionId,
				UserId = userId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Put auction in status pause",
			Description = @"
			Pauses a specific active auction if requested by the seller and the auction has no bids.

			Path parameters:
			- auctionId (GUID, required): Unique identifier of the auction to pause.

			Request body:
			- userId (GUID, required): ID of the user performing the pause.")]
		[Authorize]
		[HttpPatch("auctions/{auctionId}/pause")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Put auction in status active",
			Description = @"
			Resumes a specific paused auction if requested by the seller and the new end time is in the future.

			Path parameters:
			- auctionId (GUID, required): Unique identifier of the auction to resume.

			Request body:
			- userId (GUID, required): ID of the user performing the resume.
			- endTime (DateTime, required): New end time for the auction.")]
		[Authorize]
		[HttpPatch("auctions/{auctionId}/resume")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Delete auction",
			Description = @"
			Permanently deletes a specific auction if it is currently paused and the request is made by the seller.

			Path parameters:
			- auctionId (GUID, required): Unique identifier of the auction to delete.

			Request body:
			- userId (GUID, required): ID of the user performing the deletion.")]
		[Authorize]
		[HttpDelete("auctions/{auctionId}/delete")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

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

		private readonly IWebHostEnvironment _webHostEnvironment;

		public AuctionController(IMediator mediator, IWebHostEnvironment webHostEnvironment) : base(mediator) {
			_webHostEnvironment = webHostEnvironment;
		}

		[Authorize]
		[SwaggerOperation(Summary = "Create Auction")]
		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> CreateAuction([FromForm] CreateAuctionDTO createAuctionDTO) {

			var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var uploadDirectory = Path.Combine(_webHostEnvironment.WebRootPath,
				"uploads", "auctions");

			var imageDirectories = new List<string>();
			var allowedExts = new[] { ".jpg", ".jpeg", ".png" };

			foreach (var file in createAuctionDTO.Images) {

				var fileExtension = Path.GetExtension(file.FileName).ToLower();
				if (allowedExts.Contains(fileExtension) is false ||
					file.ContentType.StartsWith("image/") is false) {
					return BadRequest($"Only allowed: {string.Join(", ", allowedExts)}");
				}

				var fileName = $"{Guid.NewGuid()}{fileExtension}";
				var fullPath = Path.Combine(uploadDirectory, fileName);

				await using var fs = System.IO.File.Create(fullPath);
				await file.CopyToAsync(fs);

				imageDirectories.Add(Path.Combine("uploads", "auctions", fileName).Replace("\\", "/"));
			}

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
	}
}

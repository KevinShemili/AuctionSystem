using Application.UseCases.Administrator.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Controllers.Common;
using WebAPI.DTOs;

namespace WebAPI.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class AdminController : AbstractController {
		public AdminController(IMediator mediator) : base(mediator) {
		}

		[Authorize(Policy = "view.user")]
		[HttpGet("users")]
		public async Task<IActionResult> ViewUsers([FromQuery] PagedParamsDTO pagedParams) {

			var query = new GetAllUsersQuery {
				Filter = pagedParams.Filter,
				PageNumber = pagedParams.PageNumber,
				PageSize = pagedParams.PageSize,
				SortBy = pagedParams.SortBy,
				SortDesc = pagedParams.SortDesc
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure) {
				return StatusCode(result.Error.Code, result.Error.Message);
			}

			return Ok(result.Value);
		}

		[Authorize(Policy = "view.user")]
		[HttpGet("user/{id}")]
		public async Task<IActionResult> ViewUser([FromRoute] Guid id) {

			var query = new GetUserQuery {
				UserId = id
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure) {
				return StatusCode(result.Error.Code, result.Error.Message);
			}

			return Ok(result.Value);
		}

		/*
				[Authorize(Policy = "edit.user")]
				[HttpPatch("user/{id}/ban")]
				public async Task<IActionResult> BanUser() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "create.user")]
				[HttpPost]
				public async Task<IActionResult> CreateAdministrator() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "create.role")]
				[HttpPost]
				public async Task<IActionResult> CreateRole() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "edit.role")]
				[HttpPost]
				public async Task<IActionResult> AssignRole() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "edit.role")]
				[HttpPost]
				public async Task<IActionResult> AssignPermission() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "edit.auction")]
				[HttpPost]
				public async Task<IActionResult> ForceCloseAuction() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "edit.auction")]
				[HttpPost]
				public async Task<IActionResult> DeleteAuction() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "view.auction")]
				[HttpPost]
				public async Task<IActionResult> ViewAuction() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "view.role")]
				[HttpPost]
				public async Task<IActionResult> ViewRoles() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "view.role")]
				[HttpPost]
				public async Task<IActionResult> ViewRole() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "view.role")]
				[HttpPost]
				public async Task<IActionResult> ViewPermissions() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}

				[Authorize(Policy = "view.role")]
				[HttpPost]
				public async Task<IActionResult> ViewPermission() {

					var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

					var query = new GetUserQuery { UserId = Guid.Parse(userId) };

					var result = await _mediator.Send(query);
					if (result.IsFailure) {
						return StatusCode(result.Error.Code, result.Error.Message);
					}
					return Ok(result.Value);
				}*/
	}
}

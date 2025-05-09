using Application.PermissionKeys;
using Application.UseCases.Administrator.Commands;
using Application.UseCases.Administrator.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Controllers.Common;
using WebAPI.DTOs;

namespace WebAPI.Controllers {
	[ApiController]
	[Route("api/[controller]")]
	public class AdminController : AbstractController {
		public AdminController(IMediator mediator) : base(mediator) {
		}

		[Authorize(Policy = PermissionKeys.ViewUser)]
		[SwaggerOperation(Summary = "Get all users")]
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

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.ViewUser)]
		[SwaggerOperation(Summary = "Get user by ID")]
		[HttpGet("users/{id}")]
		public async Task<IActionResult> ViewUser([FromRoute] Guid id) {

			var query = new GetUserQuery {
				UserId = id
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.EditUser)]
		[SwaggerOperation(Summary = "Ban a user")]
		[HttpPatch("users/{id}/ban")]
		public async Task<IActionResult> BanUser([FromRoute] Guid id, [FromBody] string reason) {

			var command = new BanUserCommand {
				UserId = id,
				Reason = reason
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.CreateUser)]
		[SwaggerOperation(Summary = "Create an admin")]
		[HttpPost("admins")]
		public async Task<IActionResult> CreateAdministrator([FromBody] CreateAdminDTO createAdminDTO) {

			var command = new CreateAdminCommand {
				Email = createAdminDTO.Email,
				FirstName = createAdminDTO.FirstName,
				LastName = createAdminDTO.LastName,
				Roles = createAdminDTO.Roles
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.CreateRole)]
		[SwaggerOperation(Summary = "Create a new role")]
		[HttpPost("roles")]
		public async Task<IActionResult> CreateRole([FromBody] CreateRoleDTO createRoleDTO) {

			var command = new CreateRoleCommand {
				Name = createRoleDTO.Name,
				Description = createRoleDTO.Description
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.EditUser)]
		[SwaggerOperation(Summary = "Assign role/s to a user")]
		[HttpPut("users/{userId}/roles")]
		public async Task<IActionResult> AssignRoles([FromRoute] Guid userId, [FromBody] List<Guid> roleIds) {

			var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var command = new AssignRoleCommand {
				UserId = userId,
				AdminId = adminId,
				RoleIds = roleIds
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.EditRole)]
		[SwaggerOperation(Summary = "Assign permission/s to a role")]
		[HttpPut("roles/{roleId}/permissions")]
		public async Task<IActionResult> AssignPermission([FromRoute] Guid roleId, [FromBody] List<Guid> permissionIds) {

			var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var command = new AssignPermissionCommand {
				AdminId = adminId,
				PermissionIds = permissionIds,
				RoleId = roleId
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.EditAuction)]
		[SwaggerOperation(Summary = "Force-End an auction no restrictions")]
		[HttpPut("auctions/{auctionId}/end")]
		public async Task<IActionResult> ForceCloseAuction([FromRoute] Guid auctionId, [FromBody] string reason) {

			var adminId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

			var command = new ForceCloseAuctionCommand {
				AdminId = adminId,
				AuctionId = auctionId,
				Reason = reason
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.DeleteAuction)]
		[SwaggerOperation(Summary = "Delete a Force-Ended auction")]
		[HttpDelete("auctions/{auctionId}")]
		public async Task<IActionResult> DeleteAuction([FromRoute] Guid auctionId) {

			var command = new ForceDeleteAuctionCommand {
				AuctionId = auctionId
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.ViewAuction)]
		[SwaggerOperation(Summary = "Get all auction details")]
		[HttpGet("auctions/{auctionId}")]
		public async Task<IActionResult> ViewAuction([FromRoute] Guid auctionId) {

			var query = new ViewAuctionAdminQuery {
				AuctionId = auctionId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.ViewRole)]
		[SwaggerOperation(Summary = "Get all roles")]
		[HttpGet("roles")]
		public async Task<IActionResult> ViewRoles([FromQuery] PagedParamsDTO pagedParams) {

			var query = new ViewRolesQuery {
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

		[Authorize(Policy = PermissionKeys.ViewRole)]
		[SwaggerOperation(Summary = "Get role by ID")]
		[HttpGet("roles/{roleId}")]
		public async Task<IActionResult> ViewRole([FromRoute] Guid roleId) {

			var query = new ViewRoleQuery {
				RoleId = roleId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.ViewRole)]
		[SwaggerOperation(Summary = "Get all permissions")]
		[HttpGet("permissions")]
		public async Task<IActionResult> ViewPermissions([FromQuery] PagedParamsDTO pagedParams) {

			var query = new ViewPermissionsQuery {
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

		[Authorize(Policy = PermissionKeys.ViewRole)]
		[SwaggerOperation(Summary = "Get permission by ID")]
		[HttpGet("permissions/{permissionId}")]
		public async Task<IActionResult> ViewPermission([FromRoute] Guid permissionId) {

			var query = new ViewPermissionQuery {
				PermissionId = permissionId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[Authorize(Policy = PermissionKeys.ViewWallet)]
		[SwaggerOperation(Summary = "Get wallet & transactions")]
		[HttpGet("wallets/{walletId}/transactions")]
		public async Task<IActionResult> ViewTransactions([FromRoute] Guid walletId) {

			var query = new ViewWalletQuery {
				WalletId = walletId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}
	}
}
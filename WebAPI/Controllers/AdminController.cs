using Application.Common.ResultPattern;
using Application.Common.Tools.Pagination;
using Application.PermissionKeys;
using Application.UseCases.Administrator.Commands;
using Application.UseCases.Administrator.DTOs;
using Application.UseCases.Administrator.Queries;
using Application.UseCases.Profile.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;
using WebAPI.Controllers.Common;
using WebAPI.DTOs;

namespace WebAPI.Controllers {
	[ApiController]
	[Route("api/admin")]
	public class AdminController : AbstractController {
		public AdminController(IMediator mediator) : base(mediator) {
		}

		[SwaggerOperation(
			Summary = "Get all users",
			Description = @"
		Retrieves a list of users in a paged format. Able to see basic information. Allows for optional filtering and sorting.
		
		Query parameters  
		- pageNumber (int, optional): Page index. Defaults to 1.  
		- pageSize (int, optional): Number of items per page. Defaults to 10.
		- filter (string, optional): Match against filter.
		- sortBy (string, optional): Field name to sort on.
		- sortDesc (bool, optional): true -> descending. false -> ascending"
		)]
		[HttpGet("users")]
		[Authorize(Policy = PermissionKeys.ViewUser)]
		[ProducesResponseType(typeof(PagedResponse<UserDTO>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Get user by ID",
			Description = @"
		Retrieves detailed information for a specific user, including roles, permissions, wallet, created auctions, and participated auctions.

		Path parameters
		- userId (GUID, required): Unique identifier of the user to retrieve.")]
		[Authorize(Policy = PermissionKeys.ViewUser)]
		[HttpGet("users/{id}")]
		[ProducesResponseType(typeof(UserDetailsDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ViewUser([FromRoute] Guid id) {

			var query = new GetUserQuery {
				UserId = id
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Ban a user",
			Description = @"
			Bans a specific user by their ID.
			1. Delete all bids placed by the user.
			2. Delete all auctions created by the user, along with any bids on those auctions.
			3. Mark the user as blocked and record the provided reason.
			4. Persist changes and broadcast a logout event to the client if they are currently connected.

			Path parameters
			- `userId` (GUID, required): Unique identifier of the user to ban.  

			Request bod  
			- `reason` (string, required): Reason for banning the user.")]
		[Authorize(Policy = PermissionKeys.EditUser)]
		[HttpPatch("users/{id}/ban")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Create an admin",
			Description = @"
			Creates a new admin user with the provided first name, last name, email, and list of role IDs.  
			Generates a random password, hashes it, & persists the new user. Password is send via email.

			Request body
			- firstName (string, required): Admin's first name.  
			- lastName (string, required): Admin's last name.  
			- email (string, required): Email address.  
			- roles (List<Guid>, required): List of role IDs to assign, at least one.  
			")]
		[Authorize(Policy = PermissionKeys.CreateUser)]
		[HttpPost("admins")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Create a new role",
			Description = @"
			Creates a new role with the specified name and description.

			Request body
			- name (string, required): Role name.  
			- description (string, required): Role description.")]
		[Authorize(Policy = PermissionKeys.CreateRole)]
		[HttpPost("roles")]
		[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Assign role/s to a user",
			Description = @"
			Assigns a set of roles to an existing administrator user.

			Request body
			- adminId (GUID, required): ID of the administrator performing the assignment.  
			- userId (GUID, required): ID of the administrator user to update.
			- roleIds (List<Guid>, required): List of role IDs to assign.")]
		[Authorize(Policy = PermissionKeys.EditUser)]
		[HttpPut("users/{userId}/roles")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status403Forbidden)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Assign permission/s to a role",
			Description = @"
			Assigns a set of permissions to an existing role.

			Request body  
			- adminId (GUID, required): ID of the administrator performing the assignment.  
			- roleId (GUID, required): ID of the role to update.  
			- permissionIds (List<Guid>, required): List of permission IDs to assign.")]
		[Authorize(Policy = PermissionKeys.EditRole)]
		[HttpPut("roles/{roleId}/permissions")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Force-End an auction no restrictions",
			Description = @"
			Force-closes a specific auction by its ID. This operation will:
			1. Unfreeze and refund any active bids.
			2. Mark the auction as ended, record the admin who closed it, and store the provided reason.
			3. Notify the seller via email, and broadcast an END-AUCTION event to the frontend.

			Path parameters:
			- auctionId (GUID, required): Unique identifier of the auction to force close.

			Request body:
			- adminId (GUID, required): ID of the administrator performing the force-close.
			- reason (string, required): Reason for force-closing the auction.")]
		[Authorize(Policy = PermissionKeys.EditAuction)]
		[HttpPut("auctions/{auctionId}/end")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Delete a Force-Ended auction",
			Description = @"
			Permanently deletes an auction that has already been force-closed.

			Path parameters:
			- auctionId (GUID, required): Unique identifier of the auction to delete.")]
		[Authorize(Policy = PermissionKeys.DeleteAuction)]
		[HttpDelete("auctions/{auctionId}")]
		[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status409Conflict)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> DeleteAuction([FromRoute] Guid auctionId) {

			var command = new ForceDeleteAuctionCommand {
				AuctionId = auctionId
			};

			var result = await _mediator.Send(command);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Get all auction details",
			Description = @"
			Retrieves comprehensive details for a specific auction.
			1. All bids with bidder information
			2. Seller details
			3. Image file paths.
			
			Differently from the other endpoint that performs the same function and that is available to a logged in user,
			this endpoint shows all the bids with their amounts.
			Path parameters:
			- auctionId (GUID, required): Unique identifier of the auction to view.")]
		[Authorize(Policy = PermissionKeys.ViewAuction)]
		[HttpGet("auctions/{auctionId}")]
		[ProducesResponseType(typeof(AuctionAdminDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ViewAuction([FromRoute] Guid auctionId) {

			var query = new ViewAuctionAdminQuery {
				AuctionId = auctionId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Get all roles",
			Description = @"
			Retrieves a paged list of roles.

			Query parameters  
			- pageNumber (int, optional): Page index. Defaults to 1.  
			- pageSize (int, optional): Number of items per page. Defaults to 10.
			- filter (string, optional): Match against filter.
			- sortBy (string, optional): Field name to sort on.
			- sortDesc (bool, optional): true -> descending. false -> ascending")]
		[Authorize(Policy = PermissionKeys.ViewRole)]
		[HttpGet("roles")]
		[ProducesResponseType(typeof(PagedResponse<RoleDTO>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Get role by ID",
			Description = @"
			Retrieves detailed information for a specific role, including its permissions.

			Path parameters:
			- roleId (GUID, required): Unique identifier of the role to view.")]
		[Authorize(Policy = PermissionKeys.ViewRole)]
		[HttpGet("roles/{roleId}")]
		[ProducesResponseType(typeof(RoleDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ViewRole([FromRoute] Guid roleId) {

			var query = new ViewRoleQuery {
				RoleId = roleId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Get all permissions",
			Description = @"
			Retrieves a paged list of permissions.

			Query parameters  
			- pageNumber (int, optional): Page index. Defaults to 1.  
			- pageSize (int, optional): Number of items per page. Defaults to 10.
			- filter (string, optional): Match against filter.
			- sortBy (string, optional): Field name to sort on.
			- sortDesc (bool, optional): true -> descending. false -> ascending")]
		[Authorize(Policy = PermissionKeys.ViewRole)]
		[HttpGet("permissions")]
		[ProducesResponseType(typeof(PagedResponse<PermissionDTO>), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

		[SwaggerOperation(
			Summary = "Get permission by ID",
			Description = @"
			Retrieves detailed information for a specific permission.

			Path parameters:
			- permissionId (GUID, required): Unique identifier of the permission to view.")]
		[Authorize(Policy = PermissionKeys.ViewRole)]
		[HttpGet("permissions/{permissionId}")]
		[ProducesResponseType(typeof(PermissionDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
		public async Task<IActionResult> ViewPermission([FromRoute] Guid permissionId) {

			var query = new ViewPermissionQuery {
				PermissionId = permissionId
			};

			var result = await _mediator.Send(query);

			if (result.IsFailure)
				return StatusCode(result.Error.Code, result.Error.Message);

			return Ok(result.Value);
		}

		[SwaggerOperation(
			Summary = "Get wallet & transactions",
			Description = @"
			Retrieves details for a specific wallet, including its balance, frozen balance, and list of transactions.

			Path parameters:
			- walletId (GUID, required): Unique identifier of the wallet to view.")]
		[Authorize(Policy = PermissionKeys.ViewWallet)]
		[HttpGet("wallets/{walletId}/transactions")]
		[ProducesResponseType(typeof(WalletDTO), StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		[ProducesResponseType(StatusCodes.Status401Unauthorized)]
		[ProducesResponseType(typeof(Error), StatusCodes.Status404NotFound)]
		[ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
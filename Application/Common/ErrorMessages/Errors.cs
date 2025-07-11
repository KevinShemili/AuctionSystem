﻿using Application.Common.ResultPattern;
using Microsoft.AspNetCore.Http;

namespace Application.Common.ErrorMessages {
	public static class Errors {

		public static readonly Error NegativeBaselinePrice = new(StatusCodes.Status400BadRequest,
			"Baseline price should be greater than 0.");

		public static readonly Error EndSmallerEqualStart = new(StatusCodes.Status400BadRequest,
			"End time should be greater than start time.");

		public static readonly Error OneOrMoreImages = new(StatusCodes.Status400BadRequest,
			"Please upload one or more images.");

		public static readonly Error AtLeastOneImage = new(StatusCodes.Status400BadRequest,
			"Auction must have at least one image.");

		public static readonly Error NotAccessibleByAdmins = new(StatusCodes.Status403Forbidden,
			"Not accessible by administrators.");

		public static Error AuctionNotFound(Guid id) => new(StatusCodes.Status404NotFound,
			$"Auction with ID: {id} does not exist in the system");

		public static readonly Error OnlyAssignRolesToAdmin = new(StatusCodes.Status400BadRequest,
			"Can only assign roles to admin.");

		public static readonly Error AuctionNotActive = new(StatusCodes.Status409Conflict,
			"Auction is not active.");

		public static readonly Error AuctionNotPaused = new(StatusCodes.Status409Conflict,
			"Auction is not in state paused.");

		public static readonly Error AuctionPaused = new(StatusCodes.Status403Forbidden,
			"Paused auctions may be viewed only by creator.");

		public static readonly Error AuctionHasBids = new(StatusCodes.Status409Conflict,
			"Cannot perform changes on this auction as it has active bids.");

		public static readonly Error BidTooLow = new(StatusCodes.Status400BadRequest,
			"Bid must be at least equal to the baseline price.");

		public static readonly Error InsufficientFunds = new(StatusCodes.Status400BadRequest,
			"Insufficient available funds to place this bid.");

		public static readonly Error EmptyRoles = new(StatusCodes.Status400BadRequest,
			"Please provide at least one role.");

		public static readonly Error InvalidRoles = new(StatusCodes.Status400BadRequest,
			"Provided role list contains invalid entries.");

		public static readonly Error IncreaseBidInsufficientFunds = new(StatusCodes.Status400BadRequest,
			"Insufficient available funds to increase this bid.");

		public static readonly Error InvalidBidAmount = new(StatusCodes.Status400BadRequest,
			"Bid amount must be greater than zero.");

		public static readonly Error EmailAlreadyExists = new(StatusCodes.Status409Conflict,
			"Email already exists.");

		public static readonly Error InvalidPermissions = new(StatusCodes.Status400BadRequest,
			"Provided permission list contains invalid entries.");

		public static readonly Error CannotBlockAnotherAdmin = new(StatusCodes.Status403Forbidden,
			"Cannot block another Administrator.");

		public static readonly Error EndBeforeDelete = new(StatusCodes.Status409Conflict,
			"Action should be ended before being deleted.");

		public static readonly Error SignInFailure = new(StatusCodes.Status401Unauthorized,
			"Please try again.");

		public static readonly Error LockedOut = new(StatusCodes.Status429TooManyRequests,
			"Account locked. Please try again later.");

		public static readonly Error InvalidToken = new(StatusCodes.Status401Unauthorized,
			"Invalid token.");

		public static readonly Error ExpiredEmailToken = new(StatusCodes.Status401Unauthorized,
			"Token expired. New email has been sent.");

		public static readonly Error AccountAlreadyVerified = new(StatusCodes.Status409Conflict,
			"Account already verified.");

		public static readonly Error AccountNotVerified = new(StatusCodes.Status403Forbidden,
			"Account not verified. Please check email.");

		public static readonly Error ServerError = new(StatusCodes.Status500InternalServerError,
			"Server error. Please contact administrator.");

		public static readonly Error Unauthorized = new(StatusCodes.Status401Unauthorized,
			"Unauthorized. Please login.");

		public static readonly Error BidderIsSeller = new(StatusCodes.Status403Forbidden,
			"Seller cannot bid on his auction.");

		public static readonly Error AdminsCannotPlaceBids = new(StatusCodes.Status403Forbidden,
			"Administrators cannot place bids.");

		public static readonly Error ChangeOnlyOwnAuctions = new(StatusCodes.Status403Forbidden,
			"You cannot alter auctions of others.");

		public static readonly Error InvalidPasswordFormat = new(StatusCodes.Status400BadRequest,
			"The password format is invalid. It must be 8-50 characters long, contain at least one digit, one uppercase letter, and one lowercase letter.");

		public static Error UserNotFound(string email) => new(StatusCodes.Status404NotFound,
			$"User with email: {email} does not exist in the system");

		public static Error BlockReason(string reason) => new(StatusCodes.Status403Forbidden, reason);

		public static readonly Error AlreadyBlocked = new(StatusCodes.Status409Conflict,
			"User is already blocked");

		public static Error UserNotFound(Guid id) => new(StatusCodes.Status404NotFound,
			$"User with ID: {id} does not exist in the system");

		public static Error RoleNotFound(Guid id) => new(StatusCodes.Status404NotFound,
			$"Role with ID: {id} does not exist in the system");

		public static Error WalletNotFound(Guid id) => new(StatusCodes.Status404NotFound,
			$"Wallet with ID: {id} does not exist in the system");

		public static Error PermissionNotFound(Guid id) => new(StatusCodes.Status404NotFound,
			$"Permission with ID: {id} does not exist in the system");

		public static readonly Error InvalidId = new(StatusCodes.Status400BadRequest,
			"Id format is invalid.");
	}
}

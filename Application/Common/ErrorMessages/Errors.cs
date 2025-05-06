using Application.Common.ResultPattern;
using Microsoft.AspNetCore.Http;

namespace Application.Common.ErrorMessages {
	public static class Errors {

		public static readonly Error NegativeBaselinePrice = new(StatusCodes.Status400BadRequest,
			"Baseline price should be greater than 0.");

		public static readonly Error PastStartTime = new(StatusCodes.Status400BadRequest,
			"Start time cannot be a time in the past.");

		public static readonly Error EndSmallerEqualStart = new(StatusCodes.Status400BadRequest,
			"End time should be greater than start time.");

		public static readonly Error OneOrMoreImages = new(StatusCodes.Status400BadRequest,
			"Please upload one or more images.");

		public static readonly Error AuctionNotFound = new(StatusCodes.Status404NotFound,
			"Auction not found.");

		public static readonly Error AuctionNotActive = new(StatusCodes.Status400BadRequest,
			"Auction is not active.");

		public static readonly Error BidTooLow = new(StatusCodes.Status400BadRequest,
			"Bid must be at least the baseline or exceed the current highest bid.");

		public static readonly Error InsufficientFunds = new(StatusCodes.Status400BadRequest,
			"Insufficient available funds to place this bid.");

		public static readonly Error IncreaseBidInsufficientFunds = new(StatusCodes.Status400BadRequest,
			"Insufficient available funds to increase this bid.");

		public static readonly Error InvalidBidAmount = new(StatusCodes.Status400BadRequest,
			"Bid amount must be greater than zero.");

		public static readonly Error EmailAlreadyExists = new(StatusCodes.Status409Conflict,
			"Email already exists.");

		public static readonly Error SignInFailure = new(StatusCodes.Status401Unauthorized,
			"Please try again.");

		public static readonly Error LockedOut = new(StatusCodes.Status429TooManyRequests,
			"Account locked. Please try again later.");

		public static readonly Error InvalidToken = new(StatusCodes.Status400BadRequest,
			"Invalid token.");

		public static readonly Error ExpiredEmailToken = new(StatusCodes.Status400BadRequest,
			"Token expired. New email has been sent.");

		public static readonly Error AccountAlreadyVerified = new(StatusCodes.Status400BadRequest,
			"Account already verified.");

		public static readonly Error AccountNotVerified = new(StatusCodes.Status400BadRequest,
			"Account not verified. Please check email.");

		public static readonly Error ServerError = new(StatusCodes.Status500InternalServerError,
			"Server error. Please contact administrator.");

		public static readonly Error Unauthorized = new(StatusCodes.Status401Unauthorized,
			"Unauthorized. Please login.");

		public static Error UserNotFound(string Email) => new(StatusCodes.Status404NotFound,
			$"User with email: {Email} does not exist in the system");

		public static Error UserNotFound(Guid id) => new(StatusCodes.Status404NotFound,
			$"User with ID: {id} does not exist in the system");
	}
}

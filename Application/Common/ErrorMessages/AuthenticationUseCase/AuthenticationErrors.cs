using Application.Common.ResultPattern;
using Microsoft.AspNetCore.Http;

namespace Application.Common.ErrorMessages.AuthenticationUseCase {
	public static class AuthenticationErrors {
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

		public static Error UserNotFound(int id) => new(StatusCodes.Status404NotFound,
			$"User with ID: {id} does not exist in the system");
	}
}

using Application.Common.ResultPattern;
using Microsoft.AspNetCore.Http;

namespace Application.Common.ErrorMessages.AuctionUsecCase {
	public static class AuctionErrors {
		public static readonly Error NegativeBaselinePrice = new(StatusCodes.Status400BadRequest,
			"Baseline price should be greater than 0.");

		public static readonly Error PastStartTime = new(StatusCodes.Status400BadRequest,
			"Start time cannot be a time in the past.");

		public static readonly Error EndSmallerEqualStart = new(StatusCodes.Status400BadRequest,
			"End time should be greater than start time.");

		public static readonly Error OneOrMoreImages = new(StatusCodes.Status400BadRequest,
			"Please upload one or more images.");
	}
}

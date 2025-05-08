using Swashbuckle.AspNetCore.Annotations;

namespace Application.DTOs {
	public class AuctionDTO {
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }
		public IEnumerable<BidDTO> Bids { get; set; }
		public IEnumerable<AuctionImageDTO> Images { get; set; }
	}

	public class AuctionImageDTO {
		[SwaggerSchema(Format = "uri")]
		public string Url { get; set; }
	}

	public class BidDTO {
		public decimal Amount { get; set; }
		public bool IsWinningBid { get; set; }
	}
}

using Swashbuckle.AspNetCore.Annotations;

namespace Application.UseCases.Administrator.DTOs {
	public class AuctionAdminDTO {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }

		[SwaggerSchema(Format = "uri")]
		public IEnumerable<string> Images { get; set; }
		public List<BidAdminDTO> Bids { get; set; }
	}

	public class BidAdminDTO {
		public Guid Id { get; set; }
		public string BidderFirstName { get; set; }
		public string BidderLastName { get; set; }
		public string BidderEmail { get; set; }
		public decimal Amount { get; set; }
		public bool IsWinningBid { get; set; }
	}
}

using Application.UseCases.Bidding.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.UseCases.Auctions.DTOs {
	public class AuctionDTO {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }

		[SwaggerSchema(Format = "uri")]
		public IEnumerable<string> Images { get; set; }
		public IEnumerable<BidDTO> Bids { get; set; }
	}
}

using Swashbuckle.AspNetCore.Annotations;

namespace Application.UseCases.Auctions.DTOs {
	public class AuctionDetailsDTO {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }
		public string SellerFirstName { get; set; }
		public string SellerLastName { get; set; }
		public string SellerEmail { get; set; }

		[SwaggerSchema(Format = "uri")]
		public IEnumerable<string> Images { get; set; }
		public IEnumerable<BiddersDTO> Bidders { get; set; }
	}

	public class BiddersDTO {
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}

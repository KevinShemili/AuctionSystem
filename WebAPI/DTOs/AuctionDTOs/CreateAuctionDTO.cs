namespace WebAPI.DTOs.AuctionDTOs {
	public class CreateAuctionDTO {
		public string Name { get; set; } = default!;
		public string Description { get; set; } = string.Empty;
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}

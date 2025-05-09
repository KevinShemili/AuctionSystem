namespace WebAPI.DTOs {
	public class UpdateAuctionDTO {
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime EndTime { get; set; }
		public IEnumerable<IFormFile> Images { get; set; }
	}
}

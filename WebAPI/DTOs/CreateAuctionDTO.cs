using System.ComponentModel.DataAnnotations;

namespace WebAPI.DTOs {
	public class CreateAuctionDTO {
		[Required]
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }
		[Required]
		public decimal BaselinePrice { get; set; }
		[Required]
		public DateTime StartTime { get; set; }
		[Required]
		public DateTime EndTime { get; set; }
		[Required]
		public List<IFormFile> Images { get; set; }
	}
}

using Microsoft.AspNetCore.Mvc;

namespace WebAPI.DTOs {
	public class UpdateAuctionDTO {
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime EndTime { get; set; }

		[FromForm(Name = "NewImages")]
		public List<IFormFile> NewImages { get; set; }

		[FromForm(Name = "RemoveImages")]
		public List<string> RemoveImages { get; set; }
	}
}

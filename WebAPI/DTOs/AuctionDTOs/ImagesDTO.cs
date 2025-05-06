using Microsoft.AspNetCore.Mvc;

namespace WebAPI.DTOs.AuctionDTOs {
	public class ImagesDTO {
		[FromForm(Name = "images")]
		public List<IFormFile> Images { get; set; }
	}
}

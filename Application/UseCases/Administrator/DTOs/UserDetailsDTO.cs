using Application.UseCases.Bidding.DTOs;
using Swashbuckle.AspNetCore.Annotations;

namespace Application.UseCases.Administrator.DTOs {
	public class UserDetailsDTO {
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public Guid? WalletId { get; set; }
		public decimal? Balance { get; set; }
		public decimal? FrozenBalance { get; set; }
		public bool IsBlocked { get; set; }
		public bool IsAdministrator { get; set; }
		public IEnumerable<RoleDTO> Roles { get; set; }
		public IEnumerable<CreatedAuctionDTO> CreatedAuctions { get; set; }
		public IEnumerable<PartecipatedAuctionDTO> ParticipatedAuctions { get; set; }
	}

	public class BaseAuctionDTO {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }

		[SwaggerSchema(Format = "uri")]
		public IEnumerable<string> Images { get; set; }
	}

	public class CreatedAuctionDTO : BaseAuctionDTO {
		public IEnumerable<BidDTO> Bids { get; set; }
	}

	public class PartecipatedAuctionDTO : BaseAuctionDTO {
		public Guid BidId { get; set; }
		public decimal BidAmount { get; set; }
		public bool IsWinningBid { get; set; }
	}
}

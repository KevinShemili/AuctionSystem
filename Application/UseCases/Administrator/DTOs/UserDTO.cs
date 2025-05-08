using Application.UseCases.Auctions.DTOs;

namespace Application.UseCases.Administrator.DTOs {
	public class UserDTO {
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public Guid? WalletId { get; set; }
		public decimal? Balance { get; set; }
		public decimal? FrozenBalance { get; set; }
		public IEnumerable<RoleDTO> Roles { get; set; }
		public IEnumerable<AuctionDTO> CreatedAuctions { get; set; }
		public IEnumerable<AuctionDTO> ParticipatedAuctions { get; set; }
	}
}

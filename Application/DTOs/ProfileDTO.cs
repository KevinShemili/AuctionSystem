namespace Application.DTOs {
	public class ProfileDTO {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public IEnumerable<AuctionDTO> CreatedAuctions { get; set; }
		public IEnumerable<AuctionDTO> ParticipatedAuctions { get; set; }
		public WalletDTO Wallet { get; set; }

	}
}

namespace Application.UseCases.Profile.DTOs {
	public class ProfileDTO {
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public Guid WalletId { get; set; }
		public decimal Balance { get; set; }
		public decimal FrozenBalance { get; set; }
		public List<OwnAuctionsDTO> OwnAuctions { get; set; }
	}

	public class OwnAuctionsDTO {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public decimal BaselinePrice { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public int Status { get; set; }
	}
}

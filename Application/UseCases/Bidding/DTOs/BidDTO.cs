namespace Application.UseCases.Bidding.DTOs {
	public class BidDTO {
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public bool IsWinningBid { get; set; }
	}
}

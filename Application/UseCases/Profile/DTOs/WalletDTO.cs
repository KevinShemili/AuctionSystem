namespace Application.UseCases.Profile.DTOs {
	public class WalletDTO {
		public Guid Id { get; set; }
		public decimal Balance { get; set; }
		public decimal FrozenBalance { get; set; }
		public IEnumerable<TransactionsDTO> Transactions { get; set; }
	}

	public class TransactionsDTO {
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public int TransactionType { get; set; }
	}
}

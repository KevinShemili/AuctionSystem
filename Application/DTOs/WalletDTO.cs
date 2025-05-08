namespace Application.DTOs {
	public class WalletDTO {
		public decimal Balance { get; set; }
		public decimal FrozenBalance { get; set; }
		public IEnumerable<TransactionsDTO> Transactions { get; set; }
	}

	public class TransactionsDTO {
		public decimal Amount { get; set; }
		public int TransactionType { get; set; }
	}
}

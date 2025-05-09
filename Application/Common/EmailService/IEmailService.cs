namespace Application.Common.EmailService {
	public interface IEmailService {
		Task SendConfirmationEmailAsync(string token, string email, CancellationToken cancellationToken = default);
		Task SendAdminRegistrationEmailAsync(string email, string password, CancellationToken cancellationToken = default);
		Task SendAuctionClosedEmailAsync(string email, string auctionName, string sellerName, string reason, CancellationToken cancellationToken);
		Task SendBidRemovedEmailAsync(string email, string auctionName, CancellationToken cancellationToken = default);
	}
}

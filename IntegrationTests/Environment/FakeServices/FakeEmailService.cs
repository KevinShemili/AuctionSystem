using Application.Common.EmailService;

namespace IntegrationTests.Environment.FakeServices {
	public class FakeEmailService : IEmailService {

		public Task SendAdminRegistrationEmailAsync(string email, string password, CancellationToken cancellationToken = default) {
			return Task.CompletedTask;
		}

		public Task SendAuctionClosedEmailAsync(string email, string auctionName, string sellerName, string reason, CancellationToken cancellationToken) {
			return Task.CompletedTask;
		}

		public Task SendBidRemovedEmailAsync(string email, string auctionName, CancellationToken cancellationToken = default) {
			return Task.CompletedTask;
		}

		public Task SendConfirmationEmailAsync(string token, string email, CancellationToken cancellationToken = default) {
			return Task.CompletedTask;
		}
	}
}

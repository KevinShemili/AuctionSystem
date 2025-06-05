namespace Infrastructure.Email.HTMLTemplates {
	public class BodyTemplate {
		public static async Task<string> VerifyEmailBody(string url, string email, string token,
			CancellationToken cancellationToken = default) {

			var assembly = typeof(EmailService).Assembly;
			var resourceName = "Infrastructure.Email.HTMLTemplates.ConfirmEmail.html";
			using var stream = assembly.GetManifestResourceStream(resourceName);

			if (stream == null) {
				throw new FileNotFoundException($"Could not find embedded resource: {resourceName}");
			}

			using var reader = new StreamReader(stream);
			var result = await reader.ReadToEndAsync(cancellationToken);

			var body = result.Replace("LINKHERE",
				$"{url}/api/auth/confirm-email?token={token}&email={email}");

			return body;
		}

		public static async Task<string> CreateAdminBody(string password, CancellationToken cancellationToken = default) {

			var assembly = typeof(EmailService).Assembly;
			var resourceName = "Infrastructure.Email.HTMLTemplates.CreateAdmin.html";
			using var stream = assembly.GetManifestResourceStream(resourceName);

			if (stream == null)
				throw new FileNotFoundException($"Could not find embedded resource: {resourceName}");

			using var reader = new StreamReader(stream);
			var result = await reader.ReadToEndAsync(cancellationToken);

			var body = result.Replace("PASSWORD-HERE", password);

			return body;
		}

		public static async Task<string> CreateBidRemovedBody(string auctionName, CancellationToken cancellationToken = default) {

			var assembly = typeof(EmailService).Assembly;
			var resourceName = "Infrastructure.Email.HTMLTemplates.BidRemovedByAdmin.html";
			using var stream = assembly.GetManifestResourceStream(resourceName);

			if (stream == null)
				throw new FileNotFoundException($"Could not find embedded resource: {resourceName}");

			using var reader = new StreamReader(stream);
			var result = await reader.ReadToEndAsync(cancellationToken);

			var body = result.Replace("AUCTION-NAME", auctionName);

			return body;
		}

		public static async Task<string> CreateAuctionRemovedBody(string auctionName, string sellerName, string reason,
			CancellationToken cancellationToken = default) {

			var assembly = typeof(EmailService).Assembly;
			var resourceName = "Infrastructure.Email.HTMLTemplates.AuctionClosedByAdmin.html";
			using var stream = assembly.GetManifestResourceStream(resourceName);

			if (stream == null)
				throw new FileNotFoundException($"Could not find embedded resource: {resourceName}");

			using var reader = new StreamReader(stream);
			var result = await reader.ReadToEndAsync(cancellationToken);

			var body = result.Replace("AUCTION-NAME", auctionName);
			body = body.Replace("REASON-HERE", reason);
			body = body.Replace("SELLER-NAME", sellerName);

			return body;
		}
	}
}

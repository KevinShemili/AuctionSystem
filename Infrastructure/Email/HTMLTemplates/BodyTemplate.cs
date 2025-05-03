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
				$"{url}/api/Authentication/confirm-email?token={token}&email={email}");

			return body;
		}
	}
}

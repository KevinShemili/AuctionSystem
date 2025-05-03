namespace Application.Common.EmailService {
	public interface IEmailService {
		Task SendConfirmationEmailAsync(string token, string email, CancellationToken cancellationToken = default);
	}
}

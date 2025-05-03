using Application.Common.EmailService;
using Application.Common.Tools.Transcode;
using Infrastructure.Email.HTMLTemplates;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace Infrastructure.Email {
	public class EmailService : IEmailService {

		private readonly IConfiguration _config;
		private readonly IHttpContextAccessor _httpContext;

		private int _port;
		private bool _useSSL;
		private bool _useStartTls;
		private string _displayName = null!;
		private string _from = null!;
		private string _host = null!;
		private string _password = null!;
		private string _username = null!;

		public EmailService(IConfiguration config,
							IHttpContextAccessor httpContext) {
			_config = config;
			_httpContext = httpContext;

			ReadAppSettings();
		}

		private void ReadAppSettings() {
			_displayName = _config["MailSettings:DisplayName"]!;
			_from = _config["MailSettings:From"]!;
			_useSSL = bool.Parse(_config["MailSettings:UseSSL"]!);
			_useStartTls = bool.Parse(_config["MailSettings:UseStartTls"]!);
			_host = _config["MailSettings:Host"]!;
			_port = int.Parse(_config["MailSettings:Port"]!);
			_password = _config["MailSettings:Password"]!;
			_username = _config["MailSettings:UserName"]!;
		}

		private async Task<bool> SendAsync(EmailData emailData, CancellationToken cancellationToken = default) {
			ArgumentNullException.ThrowIfNull(emailData);
			try {
				var mail = new MimeMessage();

				mail.From.Add(new MailboxAddress(_displayName, _from));
				mail.Sender = new MailboxAddress(_displayName, _from);
				mail.To.Add(MailboxAddress.Parse(emailData.To));

				var body = new BodyBuilder {
					HtmlBody = emailData.Body
				};
				mail.Subject = emailData.Subject;
				mail.Body = body.ToMessageBody();

				using var smtp = new SmtpClient();

				if (_useSSL) {
					await smtp.ConnectAsync(_host, _port, SecureSocketOptions.SslOnConnect, cancellationToken);
				}
				else if (_useStartTls) {
					await smtp.ConnectAsync(_host, _port, SecureSocketOptions.StartTls, cancellationToken);
				}
				else {
					await smtp.ConnectAsync(_host, _port, SecureSocketOptions.None, cancellationToken);
				}

				await smtp.AuthenticateAsync(_username, _password, cancellationToken);
				await smtp.SendAsync(mail, cancellationToken);
				await smtp.DisconnectAsync(true, cancellationToken);

				return true;
			}
			catch (Exception) {
				// Log
				throw;
			}
		}

		public async Task SendConfirmationEmailAsync(string token, string email, CancellationToken cancellationToken) {
			var encodedToken = Transcode.EncodeURL(token);

			var body = await BodyTemplate.VerifyEmailBody(GetUrl(), email, encodedToken, cancellationToken);

			_ = await SendAsync(new EmailData {
				To = email,
				Subject = "Confirm Your Email",
				Body = body
			}, cancellationToken);
		}

		private string GetUrl() {
			var url = _httpContext.HttpContext?.Request?.Host.ToString();
			var isHttps = _httpContext.HttpContext?.Request.IsHttps;

			if (isHttps.HasValue && isHttps is true)
				url = "https://" + url;
			else
				url = "http://" + url;

			return url;
		}
	}
}

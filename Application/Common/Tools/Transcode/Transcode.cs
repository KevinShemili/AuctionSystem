using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Application.Common.Tools.Transcode {

	// Used to encode and decode tokens for things like email verification.
	// Done in order to make the token more secure and less readable.
	public static class Transcode {
		public static string EncodeURL(string content) {
			return WebEncoders.Base64UrlEncode(
				Encoding.UTF8.GetBytes(content));
		}

		public static string DecodeURL(string content) {
			return Encoding.UTF8.GetString(
				WebEncoders.Base64UrlDecode(content));
		}
	}
}

namespace Application.UseCases.Authentication.Results {
	public class RefreshTokenCommandResult {
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}

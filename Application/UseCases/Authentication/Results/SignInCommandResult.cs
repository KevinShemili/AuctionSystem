namespace Application.UseCases.Authentication.Results {
	public class SignInCommandResult {
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}

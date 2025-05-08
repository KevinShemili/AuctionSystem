namespace Application.UseCases.Authentication.DTOs {
	public class SignInDTO {
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}

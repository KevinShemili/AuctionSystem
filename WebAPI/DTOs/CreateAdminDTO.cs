namespace WebAPI.DTOs {
	public class CreateAdminDTO {
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public List<Guid> Roles { get; set; }
	}
}

namespace Application.UseCases.Administrator {
	public class UserDTO {
		public Guid Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public IEnumerable<RoleDTO> Roles { get; set; }
	}

	public class RoleDTO {
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}

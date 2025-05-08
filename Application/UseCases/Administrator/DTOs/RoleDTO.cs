namespace Application.UseCases.Administrator.DTOs {
	public class RoleDTO {
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public IEnumerable<PermissionDTO> Permissions { get; set; }
	}
}

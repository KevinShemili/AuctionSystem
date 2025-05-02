using System.ComponentModel.DataAnnotations;

namespace Domain.Common {
	public abstract class EntityBase {
		[Key]
		public int Id { get; set; }
		public bool IsDeleted { get; set; }
		public DateTime? DateCreated { get; set; }
		public DateTime? DateUpdated { get; set; }
	}
}

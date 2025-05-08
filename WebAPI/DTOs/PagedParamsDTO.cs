namespace WebAPI.DTOs {
	public class PagedParamsDTO {
		public int PageNumber { get; set; } = 1;
		public int PageSize { get; set; } = 10;
		public string Filter { get; set; }
		public string SortBy { get; set; }
		public bool SortDesc { get; set; }
	}
}

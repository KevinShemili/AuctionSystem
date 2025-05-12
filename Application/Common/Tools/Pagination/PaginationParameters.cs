namespace Application.Common.Tools.Pagination {

	public class PagedResponse<T> {
		public int PageNumber { get; set; }
		public int PageSize { get; set; }
		public int TotalRecords { get; set; }
		public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);
		public List<T> Items { get; set; }
		public bool HasPreviousPage => PageNumber > 1;
		public bool HasNextPage => PageNumber < TotalPages;
	}
}

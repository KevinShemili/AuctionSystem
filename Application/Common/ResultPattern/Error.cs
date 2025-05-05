namespace Application.Common.ResultPattern {
	public sealed record Error(int Code, string Message = null) {
		public static readonly Error None = new(int.MinValue, string.Empty);
	}
}

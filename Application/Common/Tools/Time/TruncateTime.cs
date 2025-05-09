namespace Application.Common.Tools.Time {
	public static class TruncateTime {
		public static DateTime ToMinute(DateTime dateTime) {
			return new DateTime(
					dateTime.Year, dateTime.Month, dateTime.Day,
					dateTime.Hour, dateTime.Minute, 0,
					dateTime.Kind);
		}
	}
}

namespace Application.Common.Tools.Time {
	public static class TruncateTime {

		// Given a specfic time say -> 2023-10-01 12:34:56
		// Round it up to -> 2023-10-01 12:34:00
		public static DateTime ToMinute(DateTime dateTime) {
			return new DateTime(
					dateTime.Year, dateTime.Month, dateTime.Day,
					dateTime.Hour, dateTime.Minute, 0,
					dateTime.Kind);
		}
	}
}

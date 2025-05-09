namespace Application.Common.Tools.Passwords {
	public static class PasswordGenerator {

		const int length = 8;

		public static string Generate() {

			const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

			var res = new char[length];

			var rnd = new Random();

			for (int i = 0; i < length; i++)
				res[i] = valid[rnd.Next(valid.Length)];

			return new string(res);
		}
	}
}

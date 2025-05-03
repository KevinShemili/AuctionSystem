using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Tools.Hasher {
	public static class Hasher {
		const int keySize = 64;
		const int iterations = 350000;

		public static readonly string AdminHash;
		public static readonly string AdminSalt;

		static Hasher() {
			(AdminHash, AdminSalt) = HashPasword("admin");
		}

		public static (string, string) HashPasword(string password) {
			var salt = RandomNumberGenerator.GetBytes(keySize);

			var hash = Rfc2898DeriveBytes.Pbkdf2(
				Encoding.UTF8.GetBytes(password),
				salt,
				iterations,
				HashAlgorithmName.SHA512,
				keySize);

			var hashResult = Convert.ToHexString(hash);
			var saltResult = Convert.ToHexString(salt);

			return (hashResult, saltResult);
		}

		public static bool VerifyPassword(string password, string hash, string salt) {
			var saltBytes = Convert.FromHexString(salt);
			var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, iterations, HashAlgorithmName.SHA512, keySize);
			return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
		}


	}
}

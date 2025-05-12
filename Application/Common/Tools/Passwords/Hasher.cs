using System.Security.Cryptography;
using System.Text;

namespace Application.Common.Tools.Passwords {

	// The point of the class is to provide hashing 
	// of user passwords producing the corresponding hash and salt.
	// Thus, safely securing the password in the database.
	public static class Hasher {

		// Size in bytes of the hash and salt
		const int keySize = 64;

		// Number of iterations to use.
		// The higher the number, the more secure the hash, but slower.
		const int iterations = 350000;

		// For seeding purposes
		public static readonly string AdminHash;
		public static readonly string AdminSalt;

		static Hasher() {
			(AdminHash, AdminSalt) = HashPasword("admin");
		}

		public static (string, string) HashPasword(string password) {

			// Generate a random salt
			var salt = RandomNumberGenerator.GetBytes(keySize);

			// Hash the password with the salt
			var hash = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations,
				HashAlgorithmName.SHA512, keySize);

			// Convert the hash and salt to strings
			var hashResult = Convert.ToHexString(hash);
			var saltResult = Convert.ToHexString(salt);

			return (hashResult, saltResult);
		}

		// This method is used to verify that a password is correct. 
		public static bool VerifyPassword(string password, string hash, string salt) {
			var saltBytes = Convert.FromHexString(salt);
			var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, iterations, HashAlgorithmName.SHA512, keySize);
			return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
		}
	}
}

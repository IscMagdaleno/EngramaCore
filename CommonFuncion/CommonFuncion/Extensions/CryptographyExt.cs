using System.Security.Cryptography;
using System.Text;

namespace CommonFuncion.Extensions
{
	public static class CryptographyExt
	{
		public static string Encrypt(this string plainText, string key)
		{
			using AesGcm aesAlg = new(Encoding.UTF8.GetBytes(key));
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			byte[] nonce = new byte[12];
			byte[] cipherText = new byte[plainTextBytes.Length];

			aesAlg.Encrypt(nonce, plainTextBytes, cipherText, null);

			return Convert.ToBase64String(cipherText);
		}

		public static string Decrypt(this string cipherText, string key)
		{
			using AesGcm aesAlg = new(Encoding.UTF8.GetBytes(key));
			byte[] cipherBytes = Convert.FromBase64String(cipherText);
			byte[] nonce = new byte[12];
			byte[] plainTextBytes = new byte[cipherBytes.Length];

			aesAlg.Decrypt(nonce, cipherBytes, null, plainTextBytes);

			return Encoding.UTF8.GetString(plainTextBytes);
		}
	}
}
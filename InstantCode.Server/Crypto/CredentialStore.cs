using System.Security.Cryptography;
using System.Text;

namespace InstantCode.Server.Crypto
{
    public class CredentialStore
    {
        public static byte[] KeyHash;

        public static void Store(string password)
        {
            var keyBytes = Encoding.UTF8.GetBytes(password);
            using (var sha512 = SHA512.Create())
                using (var sha256 = SHA256.Create())
                    KeyHash = sha256.ComputeHash(sha512.ComputeHash(keyBytes));
        }

    }
}

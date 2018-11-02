using System.IO;
using System.Security.Cryptography;

namespace InstantCode.Protocol.Crypto
{
    public class PacketCrypto
    {
        private static byte[] ReadToEnd(Stream stream)
        {
            var buffer = new byte[1024];
            using (var outputStream = new MemoryStream()) {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    outputStream.Write(buffer, 0, read);
                return outputStream.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] encrypted, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new CryptographicException("Failed to initialize cryptographic algorithm");

                var transform = aes.CreateDecryptor(key, iv);

                using (var inputStream = new MemoryStream(encrypted))
                using (var cryptoStream = new CryptoStream(inputStream, transform, CryptoStreamMode.Read))
                    return ReadToEnd(cryptoStream);
            }
        }

        public static byte[] Encrypt(byte[] plaintext, byte[] key, out byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new CryptographicException("Failed to initialize cryptographic algorithm");

                aes.GenerateIV();
                iv = aes.IV;

                var transform = aes.CreateEncryptor(key, aes.IV);

                using (var outputStream = new MemoryStream())
                {
                    using (var cryptoStream = new CryptoStream(outputStream, transform, CryptoStreamMode.Write))
                        cryptoStream.Write(plaintext, 0, plaintext.Length);
                    return outputStream.ToArray();
                }
            }
        }


    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace InstantCode.Server.Crypto
{
    public class PacketCrypto
    {
        private static byte[] ReadToEnd(Stream stream)
        {
            var buffer = new byte[1024];
            using (var outputStream = new MemoryStream()) {
                int read;
                while ((read = stream.Read(buffer)) > 0)
                    outputStream.Write(buffer, 0, read);
                return outputStream.ToArray();
            }
        }

        public static byte[] Decrypt(byte[] encrypted, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new CryptographicException("Failed to initialize cryptographic algorithm");

                var transform = aes.CreateDecryptor(CredentialStore.KeyHash, iv);

                using (var inputStream = new MemoryStream(encrypted))
                using (var cryptoStream = new CryptoStream(inputStream, transform, CryptoStreamMode.Read))
                    return ReadToEnd(cryptoStream);
            }
        }

        public static byte[] Encrypt(byte[] plaintext, out byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                if (aes == null)
                    throw new CryptographicException("Failed to initialize cryptographic algorithm");

                aes.GenerateIV();
                iv = aes.IV;

                var transform = aes.CreateEncryptor(CredentialStore.KeyHash, aes.IV);

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

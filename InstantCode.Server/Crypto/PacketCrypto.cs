using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace InstantCode.Server.Crypto
{
    public class PacketCrypto
    {
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

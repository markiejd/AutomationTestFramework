using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Core.Encrypt
{
    
    public class EncryptionHelper
    {
            #pragma warning disable CS8604 // Possible null reference argument.
        private static readonly byte[] Key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_KEY")); // 32 bytes for AES-256
            #pragma warning restore CS8604 // Possible null reference argument.
            #pragma warning disable CS8604 // Possible null reference argument.
        private static readonly byte[] IV = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_IV")); // 16 bytes for AES
            #pragma warning restore CS8604 // Possible null reference argument.

        /// <summary>
        /// supply normal string, get encrypted back 
        /// As long as BOTH ENCRYPTION_KEY and ENCRYPTION_IV are set locally and correct
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns>encrypted string</returns>
        public static string EncryptString(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                if (Environment.GetEnvironmentVariable("ENCRYPTION_IV") == null) return "UNKNOWN";
                if (Environment.GetEnvironmentVariable("ENCRYPTION_KEY") == null) return "UNKNOWN";
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        /// <summary>
        /// Supply a encrypted text
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns>unencrypted value</returns>
        public static string DecryptString(string cipherText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                if (Environment.GetEnvironmentVariable("ENCRYPTION_IV") == null) return "UNKNOWN";
                if (Environment.GetEnvironmentVariable("ENCRYPTION_KEY") == null) return "UNKNOWN";
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }

}

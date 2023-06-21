using System;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace Persistencia.Utils
{
    public class SecurityUtil
    {
        public static string Encrypt(string clearText, string encryptionKey)
        {
            using var md5 = new MD5CryptoServiceProvider();
            using var tdes = new TripleDESCryptoServiceProvider
            {
                Key = md5.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            using var transform = tdes.CreateEncryptor();
            byte[] textBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] bytes = transform.TransformFinalBlock(textBytes, 0, textBytes.Length);
            return HttpUtility.UrlEncode(Convert.ToBase64String(bytes, 0, bytes.Length));
        }

        public static string Decrypt(string cipherText, string encryptionKey)
        {
            using var md5 = new MD5CryptoServiceProvider();
            using var tdes = new TripleDESCryptoServiceProvider
            {
                Key = md5.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            using var transform = tdes.CreateDecryptor();
            byte[] cipherBytes = Convert.FromBase64String(HttpUtility.UrlDecode(cipherText));
            byte[] bytes = transform.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace Device.FingerPrint.Core.Services
{
    public interface ICrypto : IDisposable
    {
        string Encrypt(string strPlainText);

        string Decrypt(string encryptedText);

        string GetMD5Hash(string str);
    }

    public class Crypto: ICrypto, IDisposable
    {
        private readonly RijndaelManaged myRijndael = new RijndaelManaged();
        private readonly int iterations;
        private readonly byte[] salt;
        private IAppSettings appSettings;
        private bool disposed = false;

        public Crypto(IAppSettings appSettings)
        {
            myRijndael.BlockSize = 128;
            myRijndael.KeySize = 128;
            myRijndael.IV = HexStringToByteArray(appSettings.CryptoIV ?? "c3eef06037e837f4ee390bda69c05023");

            myRijndael.Padding = PaddingMode.PKCS7;
            myRijndael.Mode = CipherMode.CBC;
            iterations = 1000;
            salt = Encoding.UTF8.GetBytes(appSettings.CryptoSalt ?? "234292a9fce031f2");
            myRijndael.Key = GenerateKey(appSettings.CryptoPassword);
        }

        public string Encrypt(string strPlainText)
        {
            if (string.IsNullOrEmpty(strPlainText))
            {
                return string.Empty;
            }

            byte[] strText = new UTF8Encoding().GetBytes(strPlainText);
            ICryptoTransform transform = myRijndael.CreateEncryptor();
            byte[] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);

            return Convert.ToBase64String(cipherText);
        }

        public string Decrypt(string encryptedText)
        {
            if (string.IsNullOrEmpty(encryptedText))
            {
                return string.Empty;
            }

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptor = myRijndael.CreateDecryptor(myRijndael.Key, myRijndael.IV);
            byte[] originalBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

            return Encoding.UTF8.GetString(originalBytes);
        }

        public static byte[] HexStringToByteArray(string strHex)
        {
            dynamic r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }

        private byte[] GenerateKey(string strPassword)
        {
            var rfc2898 = new Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);

            return rfc2898.GetBytes(128 / 8);
        }

        public string GetMD5Hash(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }

            var md5 = new MD5CryptoServiceProvider();
            byte[] bytes = ASCIIEncoding.Default.GetBytes(str);
            byte[] encoded = md5.ComputeHash(bytes);

            var sb = new StringBuilder();
            for (int i = 0; i < encoded.Length; i++)
                sb.Append(encoded[i].ToString("x2"));

            return sb.ToString();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    myRijndael?.Dispose();
                }
                disposed = true;
            }
        }
    }
}

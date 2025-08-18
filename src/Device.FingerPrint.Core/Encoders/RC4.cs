using System;

namespace Device.FingerPrint.Core.Encoders
{
    /// <summary>
    ///     RC4 class for encryption and decryption.
    /// </summar
    public static class Rc4
    {
        /// <summary>
        ///     Encrypt method.
        /// </summary>
        /// <param name="key">The key for encryption.</param>
        /// <param name="value">The value.</param>
        /// <returns>Encrypted value.</returns>
        public static string Encrypt(string key, string value)
        {
            var keyCharArray = key.ToCharArray();
            var valueCharArray = value.ToCharArray();
            var keyLength = key.Length;
            var valueLength = value.Length;
            var map = new int[256];

            for (var i = 0; i < 256; i++)
            {
                map[i] = i;
            }

            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + map[i] + keyCharArray[i % keyLength]) % 256;
                var x = map[i];
                map[i] = map[j];
                map[j] = x;
            }

            var encryptedValue = string.Empty;
            int i2 = 0, j2 = 0;
            for (int y = 0; y < valueLength; y++)
            {
                i2 = (i2 + 1) % 256;
                j2 = (j2 + map[i2]) % 256;
                var x = map[i2];
                map[i2] = map[j2];
                map[j2] = x;
                encryptedValue += Convert.ToChar(valueCharArray[y] ^ map[(map[i2] + map[j2]) % 256]);
            }

            return encryptedValue;
        }

        public static string Decrypt(string key, string value)
        {
            return Encrypt(key, value);
        }
    }
}
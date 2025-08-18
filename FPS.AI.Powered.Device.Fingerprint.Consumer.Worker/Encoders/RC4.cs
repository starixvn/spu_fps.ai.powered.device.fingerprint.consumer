namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Encoders
{
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

            for (int i = 0, j = 0; i < 256; i++)
            {
                j = (j + map[i] + keyCharArray[i % keyLength]) % 256;
                var x = map[i];
                map[i] = map[j];
                map[j] = x;
            }

            var encryptedValue = string.Empty;
            for (int y = 0, i = 0, j = 0; y < valueLength; y++)
            {
                i = (i + 1) % 256;
                j = (j + map[i]) % 256;
                var x = map[i];
                map[i] = map[j];
                map[j] = x;
                encryptedValue += Convert.ToChar(valueCharArray[y] ^ map[(map[i] + map[j]) % 256]);
            }

            return encryptedValue;
        }

        public static string Decrypt(string key, string value)
        {
            return Encrypt(key, value);
        }
    }
}
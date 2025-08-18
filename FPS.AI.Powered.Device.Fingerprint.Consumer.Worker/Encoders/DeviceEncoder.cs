namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Encoders
{
    public static class DeviceEncoder
    {
        /// <summary>
        ///     Encrypt a non-empty encrypted string.
        /// </summary>
        /// <param name="plaintext">Text need to encrypt.</param>
        /// <returns>Return encrypted text.</returns>
        public static string Encrypt(string plaintext)
        {
            var encryptedText = Rc4.Encrypt(AppSettings.DeviceIdEncryptKey, plaintext);
            return Hex.Encode(encryptedText);
        }

        /// <summary>
        ///     Decrypt a non-empty encrypted string.
        /// </summary>
        /// <param name="encryptedText">Encrypted text need to DESCRYPT.</param>
        /// <returns>Return encrypted text.</returns>
        public static string Decrypt(string encryptedText)
        {
            var decryptedText = Hex.Decode(encryptedText);
            decryptedText = Rc4.Decrypt(AppSettings.DeviceIdEncryptKey, decryptedText);
            return decryptedText;
        }
    }
}
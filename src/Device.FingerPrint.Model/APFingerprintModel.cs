using System;
using System.Security.Cryptography;
using System.Text;

namespace Device.FingerPrint.Model
{
    public class APFingerprintModel
    {
        public string? Audio { get; set; }
        public string? Browser { get; set; }
        public string? Hardware { get; set; }
        public string? Preferences { get; set; }
        public string? Graphic { get; set; }
        public string? AudioCode { get; set; }
        public string? BrowserCode { get; set; }
        public string? HardwareCode { get; set; }
        public string? PreferencesCode { get; set; }
        public string? GraphicCode { get; set; }

        public void GenerateGroupHashCodes()
        {
            AudioCode = GenerateHashCode(Audio);
            BrowserCode = GenerateHashCode(Browser);
            HardwareCode = GenerateHashCode(Hardware);
            PreferencesCode = GenerateHashCode(Preferences);
            GraphicCode = GenerateHashCode(Graphic);
        }

        private static string? GenerateHashCode(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;   
            }

            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}

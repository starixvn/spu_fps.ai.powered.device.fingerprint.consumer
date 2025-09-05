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
        public string? Graphics { get; set; }
        public string? FPGroupAudioCode { get; set; }
        public string? FPGroupBrowserCode { get; set; }
        public string? FPGroupHardwareCode { get; set; }
        public string? FPGroupPreferencesCode { get; set; }
        public string? FPGroupGraphicsCode { get; set; }

        public void GenerateGroupHashCodes()
        {
            FPGroupAudioCode = GenerateHashCode(Audio);
            FPGroupBrowserCode = GenerateHashCode(Browser);
            FPGroupHardwareCode = GenerateHashCode(Hardware);
            FPGroupPreferencesCode = GenerateHashCode(Preferences);
            FPGroupGraphicsCode = GenerateHashCode(Graphics);
        }

        private static string? GenerateHashCode(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}

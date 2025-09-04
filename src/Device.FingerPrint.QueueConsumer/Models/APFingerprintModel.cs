using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Device.FingerPrint.QueueConsumer.Models
{
    public class APFingerprintModel
    {
        public string? Audio { get; set; }
        public string? Browser { get; set; }
        public string? Hardware { get; set; }
        public string? Preferences { get; set; }
        public string? Graphics { get; set; }
        public string? FPPatternCode01 { get; set; }
        public string? FPPatternCode02 { get; set; }

        public void GeneratePatternCode01()
        {
            var data = new
            {
                Audio,
                Browser,
                Hardware,
                Preferences
            };

            FPPatternCode01 = GenerateHashFromObject(data);
        }

        public void GeneratePatternCode02()
        {
            var data = new
            {
                Audio,
                Browser,
                Hardware,
                Preferences,
                Graphics
            };

            FPPatternCode02 = GenerateHashFromObject(data);
        }

        public void GenerateAllPatternCodes()
        {
            GeneratePatternCode01();
            GeneratePatternCode02();
        }

        // Methods to calculate hash on demand
        public string? GetAudioHash() => GenerateHashCode(Audio);
        public string? GetBrowserHash() => GenerateHashCode(Browser);
        public string? GetHardwareHash() => GenerateHashCode(Hardware);
        public string? GetPreferencesHash() => GenerateHashCode(Preferences);
        public string? GetGraphicsHash() => GenerateHashCode(Graphics);

        private static string? GenerateHashCode(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            return Convert.ToHexStringLower(hashBytes);
        }

        private static string GenerateHashFromObject(object data)
        {
            string json = JsonSerializer.Serialize(data);
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexStringLower(hashBytes);
        }
    }
}
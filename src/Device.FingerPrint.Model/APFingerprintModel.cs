using System;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Device.FingerPrint.Model
{
    public class APFingerprintModel
    {

        [JsonProperty("au")]
        public string? Au { get; set; }

        [JsonProperty("br")]
        public string? Br { get; set; }

        [JsonProperty("hw")]
        public string? Hw { get; set; }

        [JsonProperty("pf")]
        public string? Pf { get; set; }

        [JsonProperty("gp")]
        public string? Gp { get; set; }

        public string? AudioCode { get; set; }
        public string? BrowserCode { get; set; }
        public string? HardwareCode { get; set; }
        public string? PreferencesCode { get; set; }
        public string? GraphicCode { get; set; }

        public void GenerateGroupHashCodes()
        {
            AudioCode = GenerateHashCode(Au);
            BrowserCode = GenerateHashCode(Br);
            HardwareCode = GenerateHashCode(Hw);
            PreferencesCode = GenerateHashCode(Pf);
            GraphicCode = GenerateHashCode(Gp);
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

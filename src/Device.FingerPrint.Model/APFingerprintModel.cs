using System;
using System.Text;
using Newtonsoft.Json;
using Murmur;

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

            // Use MurmurHash3 to generate hash
            var murmur = MurmurHash.Create32();
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            byte[] hashBytes = murmur.ComputeHash(inputBytes);
            
            // Convert to hex string (8 characters for 32-bit hash)
            string hexHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            
            // Pad to 32 characters by repeating the hash using StringBuilder
            var sb = new StringBuilder(hexHash);
            while (sb.Length < 32)
            {
                sb.Append(hexHash);
            }
            
            // Ensure exactly 32 characters
            return sb.ToString()[..32];
        }
    }
}

using System.Text.Json.Serialization;

namespace Device.FingerPrint.Model
{
    public class DeviceInfoModel
    {
        [JsonPropertyName("tag")]
        public string? Tagging { get; set; }

        public string? F4 { get; set; }

        public string? F5 { get; set; }

        public string? Rtc { get; set; }

        [JsonPropertyName("v")]
        public string? Version { get; set; }
    }
}
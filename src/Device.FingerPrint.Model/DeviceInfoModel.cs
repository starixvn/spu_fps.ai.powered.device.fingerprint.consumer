using Newtonsoft.Json;

namespace Device.FingerPrint.Model
{
    public class DeviceInfoModel
    {
        [JsonProperty("tag")]
        public string? Tagging { get; set; }

        [JsonProperty("f4")]
        public string? F4 { get; set; }

        [JsonProperty("f5")]
        public string? F5 { get; set; }

        [JsonProperty("rtc")]
        public string? Rtc { get; set; }

        [JsonProperty("v")]
        public string? Version { get; set; }

        [JsonProperty("sk")]
        public string? SessionKey { get; set; }
    }
}

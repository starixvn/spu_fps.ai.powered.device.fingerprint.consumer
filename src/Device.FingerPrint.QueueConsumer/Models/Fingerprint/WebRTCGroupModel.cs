using System.Collections.Generic;

namespace Device.FingerPrint.QueueConsumer.Models.Fingerprint
{
    public class WebRtcGroupModel
    {
        public List<string> LocalIPs { get; set; } = new List<string>();
        public List<string> PublicIPs { get; set; } = new List<string>();
        public List<string> RelayIPs { get; set; } = new List<string>();
        public WebRtcFlagsModel? Flags { get; set; }
        public string? Error { get; set; }
        public string? Timestamp { get; set; }
    }

    public class WebRtcFlagsModel
    {
        public bool IsSupported { get; set; }
        public bool IsBlocked { get; set; }
        public bool HasNoise { get; set; }
        public bool RelayOnly { get; set; }
    }

    public class CollectedDeviceInfoModel
    {
        public string? SessionKey { get; set; }
        public string? DeviceId { get; set; }
        public string? F4Hashed { get; set; }
        public string? F5Hashed { get; set; }
        public WebRtcGroupModel? Webrtc { get; set; }
    }
}

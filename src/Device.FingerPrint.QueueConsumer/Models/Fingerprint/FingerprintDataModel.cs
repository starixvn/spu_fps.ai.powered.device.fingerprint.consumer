namespace Device.FingerPrint.QueueConsumer.Models.Fingerprint
{
    public class FingerprintDataModel
    {
        public HardwareGroupModel? HardwareFingerprint { get; set; }
        public GraphicsGroupModel? GraphicsFingerprint { get; set; }
        public AudioGroupModel? AudioFingerprint { get; set; }
        public BrowserGroupModel? BrowserFingerprint { get; set; }
        public PreferencesGroupModel? PreferencesFingerprint { get; set; }
        public WebRtcGroupModel? WebRTCFingerprint { get; set; }
        public string? Timestamp { get; set; }
    }
}

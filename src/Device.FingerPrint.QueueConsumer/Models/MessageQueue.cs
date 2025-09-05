namespace Device.FingerPrint.TransactionAPI.Model
{
    using Device.FingerPrint.Model;
    using Device.FingerPrint.QueueConsumer.Models;
    using System;

    [Serializable]
    public class MessageQueue
    {
        public string? Account { get; set; }

        public string? Action { get; set; }

        public string? ActionResult { get; set; }

        public DeviceModel? Device { get; set; }

        public string? IP { get; set; }

        public string? FakeIP { get; set; }

        public string? Subscriber { get; set; }

        public int SubscriberId { get; set; }

        public DateTime TransDate { get; set; }

        public string? BotD { get; set; }

        public MobileModel? Mobile { get; set; }

        public string? WebRTCIP { get; set; }

        public string? FPPatternCode01 { get; set; }

        public string? FPPatternCode02 { get; set; }

        public APFingerprintModel FingerPrint { get; set; } = new APFingerprintModel();
    }
}

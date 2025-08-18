namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Models
{
    public class InsertFPFingerprintEntity
    {
        public string FPFingerPrintCode { get; set; }

        public FPFingerprintAttributeModel FPFingerPrintAttribute { get; set; }

        public string FPFingerPrintExtraCode { get; set; }

        public FPFingerprintAttributeExtraModel FPFingerPrintExtraAttribute { get; set; }

        public string FPFingerPrintOldCode { get; set; }
    }
}

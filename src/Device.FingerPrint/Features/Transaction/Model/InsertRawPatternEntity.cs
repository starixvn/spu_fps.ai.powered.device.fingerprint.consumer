namespace Device.FingerPrint.Features.Transaction.Model
{
    public class InsertRawFingerprintEntity
    {
        public string? FPPatternCode01 { get; set; }

        public string? FPPatternCode02 { get; set; }

        public string? FPGroupHardwareCode { get; set; }

        public string? FPGroupGraphicsCode { get; set; }

        public string? FPGroupAudioCode { get; set; }

        public string? FPGroupBrowserCode { get; set; }

        public string? FPGroupPreferencesCode { get; set; }

        public string? FPGroupHardwareDetails { get; set; }

        public string? FPGroupGraphicsDetails { get; set; }

        public string? FPGroupAudioDetails { get; set; }

        public string? FPGroupBrowserDetails { get; set; }

        public string? FPGroupPreferencesDetails { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}

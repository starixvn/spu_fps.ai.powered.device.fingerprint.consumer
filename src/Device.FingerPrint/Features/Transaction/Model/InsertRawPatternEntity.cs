namespace Device.FingerPrint.Features.Transaction.Model
{
    public class InsertRawFingerprintEntity
    {
        public string? FPPatternCode01 { get; set; }

        public string? FPPatternCode02 { get; set; }

        public string? HardwareCode { get; set; }

        public string? GraphicCode { get; set; }

        public string? AudioCode { get; set; }

        public string? BrowserCode { get; set; }

        public string? PreferencesCode { get; set; }

        public string? HardwareDetails { get; set; }

        public string? GraphicDetails { get; set; }

        public string? AudioDetails { get; set; }

        public string? BrowserDetails { get; set; }

        public string? PreferencesDetails { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}

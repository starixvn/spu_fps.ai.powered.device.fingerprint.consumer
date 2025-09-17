namespace Device.FingerPrint.Model
{
    public class FPFingerprintAttributeModel
    {
        public string? ApplePay { get; set; }

        public string? Architecture { get; set; }

        public string? Audio { get; set; }

        public string? AudioBaseLatency { get; set; }

        public string? Canvas { get; set; }

        public string? ColorDepth { get; set; }

        public string? ColorGamut { get; set; }

        public string? Contrast { get; set; }

        public string? CookiesEnabled { get; set; }

        public string? CpuClass { get; set; }

        public string? DeviceMemory { get; set; }

        public string? DomBlockers { get; set; }

        public string? DoNotTrack { get; set; }

        public string? FontPreferences { get; set; }

        public string? Fonts { get; set; }

        public string? ForcedColors { get; set; }

        public string? HardwareConcurrency { get; set; }

        public string? Hdr { get; set; }

        public string? IndexedDB { get; set; }

        public string? InvertedColors { get; set; }

        public string? Languages { get; set; }

        public string? LocalStorage { get; set; }

        public string? Math { get; set; }

        public string? Monochrome { get; set; }

        public string? OpenDatabase { get; set; }

        public string? OsCpu { get; set; }

        public string? PdfViewerEnabled { get; set; }

        public string? Platform { get; set; }

        public string? Plugins { get; set; }

        public string? PrivateClickMeasurement { get; set; }

        public string? ReducedMotion { get; set; }

        public string? ReducedTransparency { get; set; }

        public string? ScreenFrame { get; set; }

        public string? ScreenResolution { get; set; }

        public string? SessionStorage { get; set; }

        public string? Timezone { get; set; }

        public string? TouchSupport { get; set; }

        public string? Vendor { get; set; }

        public string? VendorFlavors { get; set; }

        public string? WebGlBasics { get; set; }

        public string? WebGlExtensions { get; set; }
    }

    public class FPFingerprintAttributeExtraModel
    {
        public string? Accept { get; set; }

        public string? Encoding { get; set; }

        public string? Headers { get; set; }

        public string? OSName { get; set; }

        public string? BrowserName { get; set; }

        public string? BrowserVersion { get; set; }

        public string? UserAgent { get; set; }

        public string? Platform { get; set; }

        // Fingerprint Old
        public string? Navigator { get; set; }
        
        public string? TimezoneOffset { get; set; }
        
        public int DeviceType { get; set; }
    }

    public class FPOldFingerprintAttributeModel
    {
        public string? Navigator { get; set; }

        public string? Plugins { get; set; }

        public string? UserAgent { get; set; }

        public string? Fonts { get; set; }

        public string? ScreenResolution { get; set; }

        public string? ScreenFrame { get; set; }

        public string? TimezoneOffset { get; set; }

        public string? Canvas { get; set; }

        public string? WebGlExtensions { get; set; }

        public string? Languages { get; set; }

        public string? CookiesEnabled { get; set; }

        public string? DoNotTrack { get; set; }

        public string? SuperCookies { get; set; }

        public string? Browser { get; set; }

        public int DeviceType { get; set; }
    }

}

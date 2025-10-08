using Device.FingerPrint.Model;
using FileHelpers;
using System.Numerics;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class InsertRawTransactionEntity
    {
        public string? LoginName { get; set; }

        public string? SubscriberName { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime TransTime { get; set; }

        [FieldConverter(ConverterKind.Date, "yyyy-MM-dd HH:mm:ss.ffff")]
        public DateTime CreatedDate { get; set; }

        public string? DeviceCode { get; set; }

        public string? FingerprintCode { get; set; }

        public string? FingerprintMoreInfo { get; set; }

        public string? UserAgent { get; set; }

        public string? IP { get; set; }

        public string? FakeIP { get; set; }

        [FieldConverter(typeof(ConvertBigInteger))]
        public BigInteger? IPId { get; set; }

        [FieldHidden]
        public bool IsIPV6 { get; set; }

        public int PluginID { get; set; }

        public string? URL { get; set; }

        public string? Action { get; set; }

        public string? ActionResult { get; set; }

        public string? InvalidDevice { get; set; }

        public int TransStatus { get; set; }

        public string? FlaggedCode { get; set; }

        public string? BotDetection { get; set; }

        public string? BotComponent { get; set; }

        public string? ChallengeCode { get; set; }

        public bool? IsIncognitoMode { get; set; }

        public string? MBDeviceCodeMediaDRM { get; set; }

        public string? MBDeviceCodeMachine { get; set; }

        public string? MBDeviceCodeTagging { get; set; }

        public bool MBIsMobileApp { get; set; }

        public bool MBIsEmulator { get; set; }

        public string? MBModelName { get; set; }

        public string? MBManufacturer { get; set; }

        public string? MBBrand { get; set; }

        public string? MBSDKVersion { get; set; }

        public string? MBOS { get; set; }

        public string? MBTimezone { get; set; }

        public string? MBLanguage { get; set; }

        public string? MBCountry { get; set; }

        public string? MBDeviceDetails { get; set; }

        public string? WebRTCIPCode { get; set; }

        public string? FPPatternCode01 { get; set; }

        public string? FPPatternCode02 { get; set; }

        public string? Version { get; set; }

        public string? FPVersion { get; set; }

        public int TransFlow { get; set; }
    }
}
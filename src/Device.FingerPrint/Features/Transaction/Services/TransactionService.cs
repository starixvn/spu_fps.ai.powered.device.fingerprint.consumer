using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Model;
using Newtonsoft.Json;

namespace Device.FingerPrint.Features.Transaction.Services
{
    public interface ITransactionService
    {
        IEnumerable<FPAttributeEntity> GetFingerprintAttribute();

        void InsertRawFingerprint(IEnumerable<RawTransactionModel> transactions);

        void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions);
    }

    public class TransactionService(ITransactionDataAccess transactionDataAccess) : ITransactionService
    {
        private readonly ITransactionDataAccess transactionDataAccess = transactionDataAccess;

        public IEnumerable<FPAttributeEntity> GetFingerprintAttribute()
        {
            return transactionDataAccess.GetFingerprintAttribute();
        }

        public void InsertRawFingerprint(IEnumerable<RawTransactionModel> transactions)
        {
            var rawFPDetailJson = JsonConvert.SerializeObject(TransformInsertRawFingerprint(transactions));

            transactionDataAccess.InsertRawFingerprint(rawFPDetailJson);
        }

        public void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions)
        {
            var transJson = JsonConvert.SerializeObject(TransformInsertRawTransaction(transactions));

            transactionDataAccess.InsertRawTransaction(transJson);
        }

        private static IEnumerable<InsertRawTransactionEntity> TransformInsertRawTransaction(IEnumerable<RawTransactionModel> transactions)
        {
            return transactions.Select(transaction => new InsertRawTransactionEntity
            {
                Action = transaction.Action,
                ActionResult = transaction.ActionResult,
                BotComponent = transaction.BotComponent,
                BotDetection = transaction.BotDetection,
                CreatedDate = transaction.CreatedDate,
                DeviceCode = transaction.DeviceCode,
                FakeIP = transaction.FakeIP,
                FlaggedCode = transaction.FlaggedCode,
                InvalidDevice = transaction.InvalidDevice,
                IP = transaction.IP,
                IPId = transaction.IPId,
                IsIPV6 = transaction.IsIPV6,
                LoginName = transaction.LoginName,
                PluginID = transaction.PluginID,
                SubscriberName = transaction.SubscriberName,
                TransStatus = transaction.TransStatus,
                TransTime = transaction.TransTime,
                URL = transaction.URL,
                UserAgent = transaction.UserAgent,
                IsIncognitoMode = transaction.IsIncognitoMode,
                MBDeviceCodeMediaDRM = transaction.MBDeviceCodeMediaDRM,
                MBDeviceCodeMachine = transaction.MBDeviceCodeMachine,
                MBDeviceCodeTagging = transaction.MBDeviceCodeTagging,
                MBIsMobileApp = transaction.MBIsMobileApp,
                MBIsEmulator = transaction.MBIsEmulator,
                MBModelName = transaction.MBModelName,
                MBManufacturer = transaction.MBManufacturer,
                MBBrand = transaction.MBBrand,
                MBSDKVersion = transaction.MBSDKVersion,
                MBOS = transaction.MBOS,
                MBTimezone = transaction.MBTimezone,
                MBLanguage = transaction.MBLanguage,
                MBCountry = transaction.MBCountry,
                MBDeviceDetails = transaction.MBDeviceDetails,
                WebRTCIP = transaction.WebRTCIP,
                FPPatternCode01 = transaction.FPPatternCode01,
                FPPatternCode02 = transaction.FPPatternCode02
            });
        }

        private static IEnumerable<InsertRawFingerprintEntity> TransformInsertRawFingerprint(IEnumerable<RawTransactionModel> transactions)
        {
            return transactions
                .Where(t => t.FingerPrint != null &&
                           !string.IsNullOrEmpty(t.FPPatternCode01) &&
                           !string.IsNullOrEmpty(t.FPPatternCode02))
                .GroupBy(t => new
                {
                    t.FingerPrint.AudioCode,
                    t.FingerPrint.BrowserCode,
                    t.FingerPrint.GraphicCode,
                    t.FingerPrint.HardwareCode,
                    t.FingerPrint.PreferencesCode,
                    t.FPPatternCode01,
                    t.FPPatternCode02
                })
                .Select(static group => new InsertRawFingerprintEntity
                {
                    AudioCode = group.Key.AudioCode,
                    BrowserCode = group.Key.BrowserCode,
                    GraphicCode = group.Key.GraphicCode,
                    HardwareCode = group.Key.HardwareCode,
                    PreferencesCode = group.Key.PreferencesCode,
                    FPPatternCode01 = group.Key.FPPatternCode01,
                    FPPatternCode02 = group.Key.FPPatternCode02,
                    AudioDetails = group.First().FingerPrint.Audio,
                    BrowserDetails = group.First().FingerPrint.Browser,
                    GraphicDetails = group.First().FingerPrint.Graphic,
                    HardwareDetails = group.First().FingerPrint.Hardware,
                    PreferencesDetails = group.First().FingerPrint.Preferences,
                    CreatedDate = group.Min(t => t.TransTime.Date)
                });
        }
    }
}

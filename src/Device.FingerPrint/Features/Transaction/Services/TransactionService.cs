using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Model;
using Newtonsoft.Json;

namespace Device.FingerPrint.Features.Transaction.Services
{
    public interface ITransactionService
    {

        void InsertRawFingerprint(IEnumerable<RawTransactionModel> transactions);

        void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions);

        void InsertWebRTCIP(IEnumerable<RawTransactionModel> transactions);
    }

    public class TransactionService(ITransactionDataAccess transactionDataAccess) : ITransactionService
    {
        private readonly ITransactionDataAccess transactionDataAccess = transactionDataAccess;
        private const int DETAILS_TYPE_WEBRTCIP = 1;

        public void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions)
        {
            var transJson = JsonConvert.SerializeObject(TransformInsertRawTransaction(transactions));

            transactionDataAccess.InsertRawTransaction(transJson);
        }

        public void InsertRawFingerprint(IEnumerable<RawTransactionModel> transactions)
        {
            var rawFPDetailJson = JsonConvert.SerializeObject(TransformInsertRawFingerprint(transactions));

            transactionDataAccess.InsertRawFingerprint(rawFPDetailJson);
        }

        public void InsertWebRTCIP(IEnumerable<RawTransactionModel> transactions)
        {
            var webRTCIP = GetWebRTCIPUnique(transactions);
            var json = JsonConvert.SerializeObject(webRTCIP);

            transactionDataAccess.InsertWebRTCIP(DETAILS_TYPE_WEBRTCIP, json);
        }

        private static IEnumerable<InsertRawTransactionEntity> TransformInsertRawTransaction(IEnumerable<RawTransactionModel> transactions)
        {
            return transactions.Select(transaction => new InsertRawTransactionEntity
            {
                Action = transaction.Action,
                ActionResult = transaction.ActionResult,
                BotComponent = transaction.BotComponent,
                BotDetection = transaction.BotDetection,
                ChallengeCode = transaction.ChallengeCode,
                CreatedDate = transaction.CreatedDate,
                DeviceCode = transaction.DeviceCode,
                FakeIP = transaction.FakeIP,
                FingerprintCode = transaction.FingerprintCode,
                FingerprintMoreInfo = transaction.FingerprintMoreInfo,
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
                WebRTCIPCode = transaction.WebRTCIP?.Code,
                FPPatternCode01 = transaction.FPPatternCode01,
                FPPatternCode02 = transaction.FPPatternCode02,
                FPVersion = transaction.FPVersion,
                TransFlow = transaction.TransFlow
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
                    AudioDetails = group.First().FingerPrint.Au,
                    BrowserDetails = group.First().FingerPrint.Br,
                    GraphicDetails = group.First().FingerPrint.Gp,
                    HardwareDetails = group.First().FingerPrint.Hw,
                    PreferencesDetails = group.First().FingerPrint.Pf,
                    CreatedDate = group.Min(t => t.TransTime.Date)
                });
        }

        private static IEnumerable<object> GetWebRTCIPUnique(IEnumerable<RawTransactionModel> rawTransModels)
        {
            var webRTCIPs = rawTransModels
                .Where(x => x.WebRTCIP != null && !string.IsNullOrEmpty(x.WebRTCIP.Code))
                .GroupBy(x => new { x.WebRTCIP!.Code, x.WebRTCIP.Details })
                .Select(g => new
                {
                    g.Key.Code,
                    g.Key.Details,
                    CreatedDate = g.Min(x => x.TransTime).ToString("yyyy-MM-dd")
                });

            return webRTCIPs.ToList();
        }
    }
}

using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Model;
using Newtonsoft.Json;

namespace Device.FingerPrint.Features.Transaction.Services
{
    public interface ITransactionService
    {
        void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions);

        void InsertFPUserAgent(string userAgentJson);

        void InsertFPFingerPrint(string fingerPrintJson);
    }

    public class TransactionService(ITransactionDataAccess transactionDataAccess) : ITransactionService
    {
        private readonly ITransactionDataAccess transactionDataAccess = transactionDataAccess;

        public void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions)
        {
            var transJson = JsonConvert.SerializeObject(TransformInsertRawTransaction(transactions));

            transactionDataAccess.InsertRawTransaction(transJson);

        }

        public void InsertFPUserAgent(string userAgentJson)
        {
            transactionDataAccess.InsertFPUserAgent(userAgentJson);
        }

        public void InsertFPFingerPrint(string fingerPrintJson)
        {
            transactionDataAccess.InsertFPFingerPrint(fingerPrintJson);
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
                FPTagging = transaction.FPTagging,
                CreatedDate = transaction.CreatedDate,
                DeviceCode = transaction.DeviceCode,
                FakeIP = transaction.FakeIP,
                FingerprintCode = transaction.FingerprintCode,
                FingerprintMoreInfo = transaction.FingerprintMoreInfo,
                FlaggedCode = transaction.FlaggedCode,
                FPSTransId = transaction.FPSTransId,
                InvalidDevice = transaction.InvalidDevice,
                IP = transaction.IP,
                IPId = transaction.IPId,
                IsIPV6 = transaction.IsIPV6,
                LoginName = transaction.LoginName,
                PluginID = transaction.PluginID,
                FPFingerPrintCode = transaction.FPFingerPrintCode,
                FPFingerPrintExtraCode = transaction.FPFingerPrintExtraCode,
                FPFingerPrintOldCode = transaction.FPFingerPrintOldCode,
                FPUserAgentCode = transaction.FPUserAgentCode,
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
                MBDeviceDetails = transaction.MBDeviceDetails
            });
        }
    }
}

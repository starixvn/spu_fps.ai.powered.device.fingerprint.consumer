using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Model;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Model.Entity;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Model.Enum;
using Newtonsoft.Json;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Services
{
    public interface IMobileRawTransactionService
    {
        RawTransactionModel TransformRawTransaction(MessageQueue message);

        void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions);
    }

    public class MobileRawTransactionService(IAppSettings appSettings) : IMobileRawTransactionService
    {
        private readonly IAppSettings appSettings = appSettings;

        public RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            try
            {
                var rawTransModel = new RawTransactionModel
                {
                    LoginName = message.Account,
                    SubscriberName = message.Subscriber,
                    SubscriberId = message.SubscriberId,
                    TransTime = Convert.ToDateTime(message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")),
                    Action = message.Action,
                    ActionResult = message.ActionResult,
                    MBDeviceCodeMediaDRM = message.Mobile.MediaDRMCode,
                    MBDeviceCodeMachine = message.Mobile.MachineCode,
                    MBDeviceCodeTagging = message.Mobile.CustomCode,
                    MBIsMobileApp = message.Mobile.IsMobileAppTrans,
                    MBIsEmulator = message.Mobile.IsEmulator,
                    MBModelName = message.Mobile.ModelName,
                    MBManufacturer = message.Mobile.Manufacturer,
                    MBBrand = message.Mobile.Brand,
                    MBOS = message.Mobile.OS,
                    MBTimezone = message.Mobile.Timezone,
                    MBLanguage = message.Mobile.SystemLanguage,
                    MBCountry = message.Mobile.Country,
                    MBSDKVersion = message.Mobile.SdkVersion,
                    FlaggedCode = GetFlaggedCode(message)
                };

                TransformIP(message, rawTransModel);

                return rawTransModel;
            }
            catch (Exception ex)
            {
                //_ = LogService.ErrorAsync($"{appSettings.Environment} - Transform Mobile RawTransaction has error:\n {ex.Message}");
            }

            return null;
        }



        public void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions)
        {
            var transJson = JsonConvert.SerializeObject(TransformInsertRawTransaction(transactions));

            transactionDataAccess.InsertRawTransaction(transJson);

        }


        private string GetFlaggedCode(MessageQueue message)
        {
            if (string.IsNullOrEmpty(message.Mobile.MachineCode)
                && string.IsNullOrEmpty(message.Mobile.CustomCode)
                && string.IsNullOrEmpty(message.Mobile.MediaDRMCode))
            {
                return Flagged.EmptyDevice.ToString();
            }

            return Flagged.Human.ToString();
        }

        private void TransformIP(MessageQueue message, RawTransactionModel model)
        {
            var realIP = message.IP?.Trim();
            var ipConverted = realIP.Ip2Long();

            model.IP = realIP;
            model.IPId = ipConverted.IpNumber;
            model.IsIPV6 = ipConverted.IsIpV6;
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
            });
        }
    }
}

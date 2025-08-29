using Device.FingerPrint.Core;
using Device.FingerPrint.Model;
using Fanex.Logging;
using System;

namespace Device.FingerPrint.QueueConsumer.Services
{
    public interface IMobileRawTransactionService
    {
        RawTransactionModel TransformRawTransaction(MessageQueue message);
    }

    public class MobileRawTransactionService : IMobileRawTransactionService
    {
        private readonly IAppSettings appSettings;

        public MobileRawTransactionService(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

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
                    MBDeviceDetails = message.Mobile.DeviceDetails,
                };

                rawTransModel.FlaggedCode = GetFlaggedCode(message);

                TransformIP(message, rawTransModel);

                return rawTransModel;
            }
            catch (Exception ex)
            {
                _ = LogService.ErrorAsync($"{appSettings.Environment} - Transform Mobile RawTransaction has error:\n {ex.Message}");
            }

            return null;
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

    }
}

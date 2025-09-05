namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Core.Encoders;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.TransactionAPI.Model;
    using Newtonsoft.Json;
    using System;


    public interface IDeviceService
    {
        DeviceInfoModel GetDeviceInfo(string? di, MessageQueue message);
    }

    public class DeviceService : IDeviceService
    {
        public DeviceInfoModel GetDeviceInfo(string? di, MessageQueue message)
        {
            try
            {
                if (IsInvalidDeviceCode(di))
                {
                    return TransformEmptyDeviceInfo();
                }

                var deviceInfo = DecodeAndDeserializeDeviceInfo(di);
                return deviceInfo ?? TransformEmptyDeviceInfo();
            }
            catch
            {
                return TransformEmptyDeviceInfo();
            }
        }

        private static bool IsInvalidDeviceCode(string? di)
        {
            return string.IsNullOrWhiteSpace(di) ||
                   string.Equals(di, "null", StringComparison.OrdinalIgnoreCase);
        }

        private static DeviceInfoModel TransformEmptyDeviceInfo()
        {
            return new DeviceInfoModel
            {
                Tagging = string.Empty,
                F4 = string.Empty,
                F5 = string.Empty,
                Rtc = string.Empty,
                Version = string.Empty
            };
        }


        private static DeviceInfoModel? DecodeAndDeserializeDeviceInfo(string? di)
        {
            if (string.IsNullOrEmpty(di))
            {
                return null;
            }

            try
            {
                string value = Base64.Decode(di);
                return JsonConvert.DeserializeObject<DeviceInfoModel>(value, JsonSerializerSettingsFactory.Instance);
            }
            catch
            {
                return null;
            }
        }
    }
}
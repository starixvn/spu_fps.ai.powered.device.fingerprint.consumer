namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Core.Encoders;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.QueueConsumer.Models;
    using Device.FingerPrint.TransactionAPI.Model;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using System;


    public interface IDeviceService
    {
        DeviceInfoModel GetDeviceInfo(string deviceCode, MessageQueue message);
    }

    public class DeviceService : IDeviceService
    {
        public DeviceInfoModel GetDeviceInfo(string deviceCode, MessageQueue message)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(deviceCode) || string.Equals(deviceCode, "null", StringComparison.OrdinalIgnoreCase))
                {
                    return TransformEmptyDeviceInfo();
                }

                var deviceInfo = DecodeAndDeserializeDeviceInfo(deviceCode);

                return deviceInfo != null ? TransformDeviceInfo(deviceInfo) : TransformEmptyDeviceInfo();
            }
            catch
            {
                // Error logging commented out - not needed for production
                return TransformEmptyDeviceInfo();
            }
        }

        private static DeviceInfo? DecodeAndDeserializeDeviceInfo(string deviceInfo)
        {
            if (string.IsNullOrEmpty(deviceInfo))
            {
                return null;
            }

            try
            {
                string value = Base64.Decode(deviceInfo);
                return JsonConvert.DeserializeObject<DeviceInfo>(value, JsonSerializerSettingsFactory.Instance);
            }
            catch
            {
                return null;
            }
        }

        private static DeviceInfoModel TransformDeviceInfo(DeviceInfo deviceInfo)
        {
            var botD = new BotDModel();
            try
            {
                if (deviceInfo.BotD?.Result != null)
                {
                    botD = new BotDModel
                    {
                        Result = new BotDResultModel
                        {
                            Bot = deviceInfo.BotD.Result.Bot,
                            BotKind = deviceInfo.BotD.Result.BotKind
                        },
                        Detection = deviceInfo.BotD.Detection,
                        Component = deviceInfo.BotD.Component
                    };
                }
            }
            catch (Exception ex)
            {
                LogService.Error("Consumer TransformDeviceInfo: " + ex.Message + "\n" + JsonConvert.SerializeObject(deviceInfo));
            }

            return new DeviceInfoModel
            {
                AppVersion = deviceInfo.AppVersion ?? string.Empty,
                Version = deviceInfo.Version ?? string.Empty,
                CapturedDate = deviceInfo.CapturedDate ?? DateTimeOffset.Now.Ticks,
                DeviceFPCode = deviceInfo.DeviceCode ?? string.Empty,
                DeviceNo = deviceInfo.DeviceId ?? string.Empty,
                HRef = deviceInfo.HRef ?? string.Empty,
                NA = deviceInfo.NA ?? string.Empty,
                TimeZone = deviceInfo.TimeZone ?? string.Empty,
                UserAgent = deviceInfo.UserAgent ?? string.Empty,
                Screen = deviceInfo.Screen == null
                    ? new ScreenInfoModel { ColorDepth = 0, Height = 0, Width = 0 }
                    : new ScreenInfoModel
                    {
                        ColorDepth = deviceInfo.Screen.ColorDepth,
                        Height = deviceInfo.Screen.Height,
                        Width = deviceInfo.Screen.Width
                    },
                BotD = botD,
                InvalidDeviceId = false
            };
        }

        private static DeviceInfoModel TransformEmptyDeviceInfo()
        {
            return new DeviceInfoModel
            {
                AppVersion = string.Empty,
                CapturedDate = 0,
                DeviceFPCode = string.Empty,
                DeviceNo = string.Empty,
                HRef = string.Empty,
                NA = string.Empty,
                Screen = new ScreenInfoModel { ColorDepth = 0, Height = 0, Width = 0 },
                TimeZone = string.Empty,
                UserAgent = string.Empty,
                InvalidDeviceId = true,
                BotD = new BotDModel(),
                Version = string.Empty
            };
        }
    }
}
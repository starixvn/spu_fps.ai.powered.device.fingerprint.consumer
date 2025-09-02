namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Core.Encoders;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.QueueConsumer.Models;
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
                if (string.IsNullOrWhiteSpace(deviceCode) || deviceCode.ToLower(System.Globalization.CultureInfo.CurrentCulture) == "null")
                {
                    return TransformEmptyDeviceInfo();
                }

                var deviceInfo = DecodeAndDeserializeDeviceInfo(deviceCode);

                if (deviceInfo != null)
                {
                    return TransformDeviceInfo(deviceInfo);
                }
            }
            catch (Exception ex)
            {
                // Error logging commented out - not needed for production
            }

            return TransformEmptyDeviceInfo();
        }

        private DeviceInfo DecodeAndDeserializeDeviceInfo(string deviceInfo)
        {
            if (string.IsNullOrEmpty(deviceInfo))
            {
                return GenerateEmptyDeviceInfo();
            }

            string value = Base64.Decode(deviceInfo);

            return JsonConvert.DeserializeObject<DeviceInfo>(value, JsonSerializerSettingsFactory.Instance);
        }

        private static DeviceInfo GenerateEmptyDeviceInfo()
        {
            return new DeviceInfo
            {
                DeviceCode = string.Empty,
                AppVersion = string.Empty,
                TimeZone = string.Empty,
                UserAgent = string.Empty,
                DeviceId = "N/A",
                Screen = new DeviceScreen(),
                HRef = string.Empty,
                BotD = new BotD()
            };
        }

        private DeviceInfoModel TransformDeviceInfo(DeviceInfo deviceInfo)
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
                AppVersion = deviceInfo.AppVersion,
                Version = deviceInfo.Version,
                CapturedDate = deviceInfo.CapturedDate ?? DateTimeOffset.Now.Ticks,
                DeviceFPCode = deviceInfo.DeviceCode,
                DeviceNo = deviceInfo.DeviceId,
                HRef = deviceInfo.HRef,
                NA = deviceInfo.NA,
                TimeZone = deviceInfo.TimeZone,
                UserAgent = deviceInfo.UserAgent,
                Screen = deviceInfo.Screen == null
                    ? new ScreenInfoModel
                    {
                        ColorDepth = 0,
                        Height = 0,
                        Width = 0
                    }
                    : new ScreenInfoModel
                    {
                        ColorDepth = deviceInfo.Screen.ColorDepth,
                        Height = deviceInfo.Screen.Height,
                        Width = deviceInfo.Screen.Width
                    },
                BotD = botD
            };
        }

        private DeviceInfoModel TransformEmptyDeviceInfo()
        {
            return new DeviceInfoModel
            {
                AppVersion = string.Empty,
                CapturedDate = 0,
                DeviceFPCode = string.Empty,
                DeviceNo = string.Empty,
                HRef = string.Empty,
                NA = string.Empty,
                Screen = new ScreenInfoModel
                {
                    ColorDepth = 0,
                    Height = 0,
                    Width = 0
                },
                TimeZone = string.Empty,
                UserAgent = string.Empty,
                InvalidDeviceId = true,
                BotD = new BotDModel()
            };
        }
    }
}
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
        private const int maxLength = 100;
        private const string _UCBROWSER = "UCBrowser";
        private const string _MIBROWSER = "MiuiBrowser";
        private const string _OTHERTYPE = "Other";
        private const string _DESTOPTYPE = "Desktop";
        private const string _TABLETTYPE = "Tablet";
        private const string _MOBILETYPE = "Mobile";

        public DeviceInfoModel GetDeviceInfo(string deviceCode, MessageQueue message)
        {
            try
            {
                var locale = System.Globalization.CultureInfo.CurrentCulture;
                if (string.IsNullOrWhiteSpace(deviceCode))
                {
                    return TransformEmptyDeviceInfo();
                }
                else if (deviceCode.ToLower(locale) == "null")
                {
                    return TransformEmptyDeviceInfo();
                }
                else
                {
                    var deviceInfo = DecodeAndDeserializeDeviceInfo(deviceCode);
                    if (deviceInfo != null)
                    {
                        return TransformDeviceInfo(deviceInfo);
                    }
                    else
                    {
                        // deviceInfo is null, return empty info
                        return TransformEmptyDeviceInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                // Exception is ignored because fallback to empty device info is safe and expected.
                LogService.Error("GetDeviceInfo Exception: " + ex.Message);
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
                if (deviceInfo.BotD != null)
                {
                    if (deviceInfo.BotD.Result != null)
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
                    else
                    {
                        // BotD.Result is null, no action needed.
                    }
                }
                else
                {
                    // BotD is null, no action needed.
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
                InvalidDeviceId = true
            };
        }
    }
}

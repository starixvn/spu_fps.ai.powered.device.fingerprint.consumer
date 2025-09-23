namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Model;
    using Fanex.Logging;
    using System;
    using System.Collections;

    public interface IRawTransactionService
    {
        RawTransactionModel TransformRawTransaction(MessageQueue message);
    }

    public class RawTransactionService(
        IBotDetectionService botDetectionService,
        IAppSettings appSettings
        ) : IRawTransactionService
    {
        private readonly IBotDetectionService botDetectionService = botDetectionService;
        private readonly IAppSettings appSettings = appSettings;

        public RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            try
            {
                var deviceInfo = message.DeviceInfo;

                var userAgent = ProcessUserAgent(deviceInfo.UserAgent);
                var transBit = ProcessDeviceValidation(deviceInfo);
                var (botDModel, flaggedCode, botDetection) = ProcessBotDetection(message, deviceInfo, userAgent);
                var (realIP, ipConverted) = ProcessIPInfo(message);

                message.Fingerprint?.GenerateGroupHashCodes();

                return CreateRawTransactionModel(message, deviceInfo, message.Device?.Url, userAgent, transBit, botDModel, flaggedCode, botDetection, realIP, ipConverted);
            }
            catch (Exception ex)
            {
                // Log error but don't let logging issues break the flow
                try
                {
                    _ = LogService.ErrorAsync($"{appSettings.Environment} - TransformRawTransaction has error:\n {ex.Message}");
                }
                catch
                {
                    // Ignore logging errors in test environment
                }
                return null;
            }
        }

        private static string ProcessUserAgent(string? userAgent)
        {
            return Utilities.StripUnicodeCharactersFromString(Utilities.ConvertNA2Empty(userAgent)) ?? "";
        }

        private static BitArray ProcessDeviceValidation(DeviceInfoModel deviceInfo)
        {
            var transBit = new BitArray(16);

            if (string.IsNullOrWhiteSpace(deviceInfo.DeviceNo) || deviceInfo.DeviceNo == "N/A")
            {
                deviceInfo.DeviceNo = null;
                transBit.Set((int)TransStatus.DeviceCodeIsEmpty, true);
            }
            else if (!Utilities.IsGuidDeviceId(deviceInfo.DeviceNo) || deviceInfo.InvalidDeviceId)
            {
                // Keep the original device code (invalidDevice) as is, don't overwrite it
                deviceInfo.DeviceNo = null;
                transBit.Set((int)TransStatus.InvalidDI, true);
            }
            else
            {
                // Device validation passed - no action needed
            }

            return transBit;
        }

        private (BotDModel botDModel, string flaggedCode, string botDetection) ProcessBotDetection(MessageQueue message, DeviceInfoModel deviceInfo, string userAgent)
        {
            var botDModel = botDetectionService.TransformBotdModel(message);
            var flaggedCode = botDetectionService.GetFlaggedCode(botDModel, deviceInfo.DeviceNo, userAgent);
            var botDetection = botDModel?.IsVer3 == true ? botDModel?.Detection : botDetectionService.GetBotDetectionTrue(botDModel);

            return (botDModel, flaggedCode, botDetection);
        }

        private static (string? realIP, IpModel ipConverted) ProcessIPInfo(MessageQueue message)
        {
            var realIP = message.IP?.Trim();
            
            // Handle null/empty IP by returning IpModel with IpNumber = 0
            if (string.IsNullOrWhiteSpace(realIP))
            {
                return (realIP, new IpModel { IpNumber = 0, IsIpV6 = false });
            }
            
            var ipConverted = realIP.Ip2Long();
            return (realIP, ipConverted);
        }

#pragma warning disable S1541
#pragma warning disable S107
        private static RawTransactionModel CreateRawTransactionModel(
            MessageQueue message,
            DeviceInfoModel deviceInfo,
            string url,
            string userAgent,
            BitArray transBit,
            BotDModel botDModel,
            string flaggedCode,
            string botDetection,
            string? realIP,
            IpModel ipConverted)
        {
            const int DefaultPluginId = 0;

            // S107: Reduce cognitive complexity by extracting logic to local functions
            string? GetUrl()
            {
                return string.IsNullOrWhiteSpace(url)
                ? Utilities.ConvertNA2Empty(Utilities.SubStringUrl(deviceInfo.HRef)?.ToString())
                : Utilities.SubStringUrl(url)?.ToString();
            }

            string GetUserAgent()
            {
                return string.IsNullOrWhiteSpace(userAgent)
                ? Utilities.ConvertNA2Empty(Utilities.StripUnicodeCharactersFromString(deviceInfo.UserAgent)) ?? ""
                : Utilities.StripUnicodeCharactersFromString(userAgent) ?? "";
            }

            string? GetFingerprintCode()
            {
                return deviceInfo.DeviceFPCode != "N/A" ? deviceInfo.DeviceFPCode : null;
            }

            string? GetFingerprintMoreInfo()
            {
                return deviceInfo.DeviceFPCode != "N/A" ? Utilities.EncodeCapturedData(deviceInfo) : null;
            }

            return new RawTransactionModel
            {
                DeviceCode = string.IsNullOrEmpty(deviceInfo.DeviceNo) ? deviceInfo.Tagging : deviceInfo.DeviceNo,
                InvalidDevice = GetInvalidDeviceId(transBit, message.Device?.Di),
                TransStatus = Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                SubscriberId = message.SubscriberId,
                TransTime = Convert.ToDateTime(message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff", System.Globalization.CultureInfo.InvariantCulture)),
                PluginID = DefaultPluginId,
                URL = GetUrl(),
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = GetUserAgent(),
                FingerprintCode = GetFingerprintCode(),
                FingerprintMoreInfo = GetFingerprintMoreInfo(),
                FlaggedCode = flaggedCode,
                BotDetection = botDetection,
                BotComponent = botDModel?.Component ?? string.Empty,
                IP = realIP,
                IPId = ipConverted.IpNumber,
                FakeIP = message.FakeIP?.Trim(),
                IsIPV6 = ipConverted.IsIpV6,

                WebRTCIP = new WebRTCIPModel(deviceInfo.Rtc ?? string.Empty),
                FPPatternCode01 = GetTrimmedValue(deviceInfo.F4),
                FPPatternCode02 = GetTrimmedValue(deviceInfo.F5),
                FingerPrint = message.Fingerprint,
                FPVersion = deviceInfo.Version ?? string.Empty,
                TransFlow = message.IsNewFlow ? 1 : 0
            };
        }
#pragma warning restore S107
#pragma warning restore S1541


        private static string? GetInvalidDeviceId(BitArray transBit, string? di)
        {
            if (transBit.Get((int)TransStatus.InvalidDI))
            {
                return di;
            }

            return null;
        }

        private static string? GetTrimmedValue(string? value)
        {
            return value?.Trim();
        }
    }
}

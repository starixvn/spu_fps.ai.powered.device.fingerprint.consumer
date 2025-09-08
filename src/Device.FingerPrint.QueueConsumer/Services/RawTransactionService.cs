namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.QueueConsumer.Models;
    using Device.FingerPrint.TransactionAPI.Model;
    using Fanex.Logging;
    using System;
    using System.Collections;

    public interface IRawTransactionService
    {
        RawTransactionModel TransformRawTransaction(MessageQueue message);
    }

    public class RawTransactionService(
        IDeviceService deviceService,
        IBotDetectionService botDetectionService,
        IAppSettings appSettings) : IRawTransactionService
    {
        private const int TRANS_BIT_ARRAY_SIZE = 16;
        private const int DEFAULT_PLUGIN_ID = 0;
        private const string DEVICE_NOT_AVAILABLE = "N/A";
        private const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss.ffff";


        private readonly IDeviceService deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        private readonly IBotDetectionService botDetectionService = botDetectionService ?? throw new ArgumentNullException(nameof(botDetectionService));
        private readonly IAppSettings appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));

        public RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            if (message == null)
            {
                LogError("Message cannot be null");
                return null;
            }

            try
            {
                return CreateRawTransactionModel(message);
            }
            catch (Exception ex)
            {
                LogError($"TransformRawTransaction failed: {ex.Message}", ex);
                return null;
            }
        }

        private static BitArray ValidateDevice(DeviceInfoModel deviceInfo)
        {
            var transBit = new BitArray(TRANS_BIT_ARRAY_SIZE);

            if (string.IsNullOrWhiteSpace(deviceInfo.Tagging) || deviceInfo.Tagging == DEVICE_NOT_AVAILABLE)
            {
                deviceInfo.Tagging = null;
                transBit.Set((int)TransStatus.DeviceCodeIsEmpty, true);
            }
            else if (!Utilities.IsGuidDeviceId(deviceInfo.Tagging))
            {
                deviceInfo.Tagging = null;
                transBit.Set((int)TransStatus.InvalidDI, true);
            }
            else
            {
                // Device tagging is valid (GUID format) - no action needed
                // transBit remains with all flags set to false indicating valid device
            }

            return transBit;
        }

        private BotDetectionResult ProcessBotDetection(MessageQueue message, DeviceInfoModel deviceInfo, string userAgent)
        {
            var botDModel = botDetectionService.TransformBotdModel(message);
            var flaggedCode = botDetectionService.GetFlaggedCode(botDModel, deviceInfo.Tagging ?? string.Empty, userAgent);
            var botDetection = botDModel.IsVer3 ? botDModel.Detection : botDetectionService.GetBotDetectionTrue(botDModel);

            return new BotDetectionResult(botDModel, flaggedCode, botDetection);
        }

        private static IpProcessingResult ProcessIP(MessageQueue message)
        {
            var realIP = message.IP?.Trim();
            return new IpProcessingResult(realIP, realIP?.Ip2Long());
        }

        private RawTransactionModel CreateRawTransactionModel(MessageQueue message)
        {
            // Ensure message has Device object
            message.Device ??= new DeviceModel();
            
            var deviceInfo = deviceService.GetDeviceInfo(message.Device?.Di, message);
            var transBit = ValidateDevice(deviceInfo);
            
            // Set device ID if valid
            if (!string.IsNullOrWhiteSpace(deviceInfo.Tagging) && message.Device != null)
            {
                message.Device.Di = deviceInfo.Tagging;
            }
            else if (message.Device != null && string.IsNullOrWhiteSpace(message.Device.Di))
            {
                // If no valid tagging, ensure Di has some value for backward compatibility
                message.Device.Di = string.Empty;
            }
            else
            {
                // Device is null or Di already has value - no action needed
            }
            
            var userAgent = ProcessUserAgent(message.Device?.UserAgent);
            var botResult = ProcessBotDetection(message, deviceInfo, userAgent);
            var ipResult = ProcessIP(message);

            message.Fingerprint.GenerateGroupHashCodes();

            return BuildRawTransactionModel(message, deviceInfo, transBit, userAgent, botResult, ipResult);
        }

        private static string ProcessUserAgent(string? userAgent)
        {
            return Utilities.StripUnicodeCharactersFromString(Utilities.ConvertNA2Empty(userAgent));
        }

        private static Uri? ProcessUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }
            return Utilities.SubStringUrl(url ?? string.Empty);
        }

        private static string? GetValidDeviceId(string? deviceId)
        {
            return string.IsNullOrWhiteSpace(deviceId) ? null : deviceId;
        }

        private static DateTime ProcessTransactionTime(DateTime transDate)
        {
            return Convert.ToDateTime(
                transDate.ToString(DATETIME_FORMAT, System.Globalization.CultureInfo.InvariantCulture));
        }

        private RawTransactionModel BuildRawTransactionModel(
            MessageQueue message,
            DeviceInfoModel deviceInfo,
            BitArray transBit,
            string userAgent,
            BotDetectionResult botResult,
            IpProcessingResult ipResult)
        {
            return new RawTransactionModel
            {
                DeviceCode = deviceInfo.Tagging,
                InvalidDevice = GetValidDeviceId(message.Device?.Di),
                TransStatus = Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                SubscriberId = message.SubscriberId,
                TransTime = ProcessTransactionTime(message.TransDate),
                PluginID = DEFAULT_PLUGIN_ID,
                URL = ProcessUrl(message.Device?.Url)?.ToString(),
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = userAgent,
                FlaggedCode = botResult.FlaggedCode,
                BotDetection = botResult.BotDetection,
                BotComponent = GetBotComponent(botResult.BotDModel),
                IP = ipResult.RealIP,
                IPId = GetIpId(ipResult.IpConverted),
                FakeIP = GetTrimmedValue(message.FakeIP),
                IsIPV6 = GetIsIPV6(ipResult.IpConverted),
                ScriptVersion = deviceInfo.Version,
                WebRTCIP = GetTrimmedValue(deviceInfo.Rtc),
                FPPatternCode01 = GetTrimmedValue(deviceInfo.F4),
                FPPatternCode02 = GetTrimmedValue(deviceInfo.F5),
                FingerPrint = message.Fingerprint
            };
        }

        private static string GetBotComponent(BotDModel? botDModel)
        {
            return botDModel?.Component ?? string.Empty;
        }

        private static long GetIpId(IpModel? ipModel)
        {
            return ipModel?.IpNumber != null ? (long)ipModel.IpNumber : 0;
        }

        private static string? GetTrimmedValue(string? value)
        {
            return value?.Trim();
        }

        private static bool GetIsIPV6(IpModel? ipModel)
        {
            return ipModel?.IsIpV6 ?? false;
        }

        // Helper methods
        private void LogError(string message, Exception? exception = null)
        {
            var fullMessage = $"{appSettings.Environment} - {message}";
            if (exception != null)
            {
                fullMessage += $"\n{exception}";
            }
            _ = LogService.ErrorAsync(fullMessage);
        }

        // Result records
        private sealed record BotDetectionResult(BotDModel BotDModel, string? FlaggedCode, string? BotDetection);
        private sealed record IpProcessingResult(string? RealIP, IpModel? IpConverted);
    }
}

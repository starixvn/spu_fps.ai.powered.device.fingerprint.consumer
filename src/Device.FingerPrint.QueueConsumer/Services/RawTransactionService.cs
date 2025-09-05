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
        RawTransactionModel? TransformRawTransaction(MessageQueue message);
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

        public RawTransactionModel? TransformRawTransaction(MessageQueue message)
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
            var deviceInfo = deviceService.GetDeviceInfo(message.Device?.Di, message);
            var transBit = ValidateDevice(deviceInfo);
            var userAgent = ProcessUserAgent(message);
            var botResult = ProcessBotDetection(message, deviceInfo, userAgent);
            var ipResult = ProcessIP(message);
            var url = ProcessUrl(message);

            message.FingerPrint?.GenerateGroupHashCodes();

            return BuildRawTransactionModel(message, deviceInfo, transBit, userAgent, botResult, ipResult, url);
        }

        private static string ProcessUserAgent(MessageQueue message)
        {
            return Utilities.StripUnicodeCharactersFromString(Utilities.ConvertNA2Empty(message.Device?.UserAgent));
        }

        private static string? ProcessUrl(MessageQueue message)
        {
            return Utilities.ConvertNA2Empty(Utilities.SubStringUrl(message.Device?.Url ?? string.Empty)?.ToString());
        }

        private static RawTransactionModel BuildRawTransactionModel(
            MessageQueue message, 
            DeviceInfoModel deviceInfo, 
            BitArray transBit, 
            string userAgent, 
            BotDetectionResult botResult, 
            IpProcessingResult ipResult, 
            string? url)
        {
            return new RawTransactionModel
            {
                DeviceCode = deviceInfo.Tagging,
                InvalidDevice = GetInvalidDevice(message.Device?.Di),
                TransStatus = Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                SubscriberId = message.SubscriberId,
                TransTime = ParseTransactionTime(message.TransDate),
                PluginID = DEFAULT_PLUGIN_ID,
                URL = url,
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = userAgent,
                FlaggedCode = botResult.FlaggedCode,
                BotDetection = botResult.BotDetection,
                BotComponent = GetBotComponent(botResult.BotDModel),
                IP = ipResult.RealIP,
                IPId = GetIpId(ipResult.IpConverted),
                FakeIP = message.FakeIP?.Trim(),
                IsIPV6 = GetIsIPV6(ipResult.IpConverted),
                ScriptVersion = deviceInfo.Version,
                WebRTCIP = deviceInfo.Rtc?.Trim(),
                FPPatternCode01 = message.FPPatternCode01?.Trim(),
                FPPatternCode02 = message.FPPatternCode02?.Trim(),
                FingerPrint = message.FingerPrint
            };
        }

        private static string GetBotComponent(BotDModel? botDModel)
        {
            return botDModel?.Component ?? string.Empty;
        }

        private static long GetIpId(IpModel? ipModel)
        {
            return (long)(ipModel?.IpNumber ?? 0);
        }

        private static bool GetIsIPV6(IpModel? ipModel)
        {
            return ipModel?.IsIpV6 ?? false;
        }

        private static string? GetInvalidDevice(string? deviceId)
        {
            return string.IsNullOrWhiteSpace(deviceId) ? null : deviceId;
        }

        private static DateTime ParseTransactionTime(DateTime transDate)
        {
            return Convert.ToDateTime(transDate.ToString(DATETIME_FORMAT, System.Globalization.CultureInfo.InvariantCulture));
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
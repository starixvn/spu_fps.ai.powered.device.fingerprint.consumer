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

    public class RawTransactionService : IRawTransactionService
    {
        private const int TRANS_BIT_ARRAY_SIZE = 16;
        private const int DEFAULT_PLUGIN_ID = 0;
        private const string DEVICE_NOT_AVAILABLE = "N/A";
        private const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss.ffff";

        private readonly IDeviceService deviceService;
        private readonly IBotDetectionService botDetectionService;
        private readonly IAppSettings appSettings;

        public RawTransactionService(
            IDeviceService deviceService,
            IBotDetectionService botDetectionService,
            IAppSettings appSettings)
        {
            this.deviceService = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            this.botDetectionService = botDetectionService ?? throw new ArgumentNullException(nameof(botDetectionService));
            this.appSettings = appSettings ?? throw new ArgumentNullException(nameof(appSettings));
        }

        public RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            if (message == null)
            {
                LogError("Message cannot be null");
                return null;
            }

            try
            {
                var deviceProcessingResult = ProcessDeviceInfo(message);
                var deviceValidationResult = ProcessDeviceValidation(deviceProcessingResult.DeviceInfo);
                var botDetectionResult = ProcessBotDetection(message, deviceProcessingResult.DeviceInfo, deviceProcessingResult.UserAgent);
                var ipProcessingResult = ProcessIPInfo(message);

                return CreateRawTransactionModel(
                    message,
                    deviceProcessingResult,
                    deviceValidationResult,
                    botDetectionResult,
                    ipProcessingResult);
            }
            catch (Exception ex)
            {
                LogError($"TransformRawTransaction failed: {ex.Message}", ex);
                return null;
            }
        }

        private DeviceProcessingResult ProcessDeviceInfo(MessageQueue message)
        {
            var deviceInfo = new DeviceInfoModel();
            var url = string.Empty;
            var userAgent = string.Empty;
            var originalDeviceCode = string.Empty;

            if (message.Device != null)
            {
                originalDeviceCode = message.Device.Di ?? string.Empty;
                deviceInfo = deviceService.GetDeviceInfo(originalDeviceCode, message);
                url = message.Device.Url ?? string.Empty;
                userAgent = message.Device.UserAgent ?? string.Empty;
            }

            return new DeviceProcessingResult(deviceInfo, url, userAgent, originalDeviceCode);
        }

        private static BitArray ProcessDeviceValidation(DeviceInfoModel deviceInfo)
        {
            var transBit = new BitArray(TRANS_BIT_ARRAY_SIZE);

            if (IsDeviceCodeEmpty(deviceInfo.DeviceNo))
            {
                deviceInfo.DeviceNo = null;
                transBit.Set((int)TransStatus.DeviceCodeIsEmpty, true);
            }
            else if (IsDeviceCodeInvalid(deviceInfo))
            {
                deviceInfo.DeviceNo = null;
                transBit.Set((int)TransStatus.InvalidDI, true);
            }

            return transBit;
        }

        private BotDetectionResult ProcessBotDetection(MessageQueue message, DeviceInfoModel deviceInfo, string userAgent)
        {
            var botDModel = botDetectionService.TransformBotdModel(message);
            var flaggedCode = botDetectionService.GetFlaggedCode(botDModel, deviceInfo.DeviceNo ?? string.Empty, userAgent ?? string.Empty);
            var botDetection = DetermineBotDetection(botDModel);

            return new BotDetectionResult(botDModel, flaggedCode, botDetection);
        }

        private static IpProcessingResult ProcessIPInfo(MessageQueue message)
        {
            var realIP = message.IP?.Trim();
            var ipConverted = realIP?.Ip2Long();

            return new IpProcessingResult(realIP, ipConverted);
        }

        private static RawTransactionModel CreateRawTransactionModel(
            MessageQueue message,
            DeviceProcessingResult deviceResult,
            BitArray transBit,
            BotDetectionResult botResult,
            IpProcessingResult ipResult)
        {
            var fingerPrint = GenerateFingerPrintSample();
            fingerPrint?.GenerateAllPatternCodes();

            return new RawTransactionModel
            {
                DeviceCode = deviceResult.DeviceInfo.DeviceNo,
                InvalidDevice = GetInvalidDeviceCode(deviceResult.OriginalDeviceCode),
                TransStatus = Core.Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                SubscriberId = message.SubscriberId,
                TransTime = ParseTransactionTime(message.TransDate),
                PluginID = DEFAULT_PLUGIN_ID,
                URL = GetProcessedUrl(deviceResult),
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = GetProcessedUserAgent(deviceResult),
                SessionKey = message.SessionKey,
                FlaggedCode = botResult.FlaggedCode,
                BotDetection = botResult.BotDetection,
                BotComponent = botResult.BotDModel?.Component ?? string.Empty,
                IP = ipResult.RealIP,
                IPId = ipResult.IpConverted?.IpNumber ?? 0,
                FakeIP = message.FakeIP?.Trim(),
                IsIPV6 = ipResult.IpConverted?.IsIpV6 ?? false,
                ScriptVersion = deviceResult.DeviceInfo.Version,
                WebRTCIP = message.WebRTCIP?.Trim(),
                FPPatternCode01 = fingerPrint?.FPPatternCode01,
                FPPatternCode02 = fingerPrint?.FPPatternCode02
            };
        }

        private static APFingerprintModel GenerateFingerPrintSample()
        {
            return new APFingerprintModel
            {
                Audio = "{\"sampleRate\":44100,\"channelCount\":2,\"fingerprint\":\"audiofp123456\"}",
                Browser = "{\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/115.0.0.0 Safari/537.36\",\"language\":\"en-US\",\"languages\":[\"en-US\",\"en\"],\"doNotTrack\":\"1\",\"cookieEnabled\":true,\"javaEnabled\":false,\"timezone\":\"Asia/Ho_Chi_Minh\"}",
                Hardware = "{\"platform\":\"Win32\",\"architecture\":\"x64\",\"oscpu\":\"Windows NT 10.0; Win64; x64\",\"cpuClass\":\"x86\",\"hardwareConcurrency\":8,\"deviceMemory\":16}",
                Preferences = "{\"darkMode\":true,\"saveData\":false,\"reducedMotion\":false}",
                Graphics = "{\"canvas\":{\"fingerprint\":\"ab12cd34ef56\",\"status\":\"success\",\"width\":300,\"height\":150},\"webglBasics\":{\"vendor\":\"Google Inc.\",\"renderer\":\"ANGLE (NVIDIA GeForce GTX 1650 Direct3D11 vs_5_0 ps_5_0)\",\"version\":\"WebGL 1.0\",\"shadingLanguageVersion\":\"WebGL GLSL ES 1.0\"},\"fonts\":{\"installed\":[\"Arial\",\"Verdana\",\"Times New Roman\"]},\"screenResolution\":{\"width\":1920,\"height\":1080},\"colorDepth\":{\"value\":24}}"
            };
        }

        // Helper methods
        private static bool IsDeviceCodeEmpty(string deviceNo) =>
            string.IsNullOrWhiteSpace(deviceNo) || deviceNo == DEVICE_NOT_AVAILABLE;

        private static bool IsDeviceCodeInvalid(DeviceInfoModel deviceInfo)
        {
            return !Utilities.IsGuidDeviceId(deviceInfo.DeviceNo ?? string.Empty) || deviceInfo.InvalidDeviceId;
        }

        private string? DetermineBotDetection(BotDModel botDModel)
        {
            return botDModel?.IsVer3 == true
                ? botDModel?.Detection
                : botDetectionService.GetBotDetectionTrue(botDModel);
        }

        private static string? GetInvalidDeviceCode(string originalDeviceCode)
        {
            return string.IsNullOrWhiteSpace(originalDeviceCode) ? null : originalDeviceCode;
        }

        private static DateTime ParseTransactionTime(DateTime transDate)
        {
            return Convert.ToDateTime(transDate.ToString(DATETIME_FORMAT, System.Globalization.CultureInfo.InvariantCulture));
        }

        private static string GetProcessedUrl(DeviceProcessingResult deviceResult)
        {
            return string.IsNullOrWhiteSpace(deviceResult.Url)
                ? Utilities.ConvertNA2Empty(Utilities.SubStringUrl(deviceResult.DeviceInfo.HRef))
                : Utilities.SubStringUrl(deviceResult.Url);
        }

        private static string GetProcessedUserAgent(DeviceProcessingResult deviceResult)
        {
            return string.IsNullOrWhiteSpace(deviceResult.UserAgent)
                ? Utilities.ConvertNA2Empty(Utilities.StripUnicodeCharactersFromString(deviceResult.DeviceInfo.UserAgent))
                : Utilities.StripUnicodeCharactersFromString(deviceResult.UserAgent);
        }

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
        private sealed record DeviceProcessingResult(DeviceInfoModel DeviceInfo, string Url, string UserAgent, string OriginalDeviceCode);
        private sealed record BotDetectionResult(BotDModel? BotDModel, string? FlaggedCode, string? BotDetection);
        private sealed record IpProcessingResult(string? RealIP, IpModel? IpConverted);
    }
}

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
                message.Device ??= new DeviceModel();
                message.Device.Di = GenDISample();

                return CreateRawTransactionModel(message);
            }
            catch (Exception ex)
            {
                LogError($"TransformRawTransaction failed: {ex.Message}", ex);
                return null;
            }
        }

        private static string GenDISample()
        {
            return "{\"sk\":\"4c6054c9ff0c3fc4b4c2d03dcbc21fa52711ffffa496ec3cca56e2790d6cd21d\",\"f4\":\"11470ce51eaaae4836ec8e8a4fc48f02\",\"tag\":\"328c9c5f6ead4e2a81f1240b80220ec9\",\"f5\":\"c56ff16e37764374e1cb25c952f6aef4\",\"rtc\":\"127.0.0.1;119.82.130.208;119.82.130.209;111.11.111.111\",\"v\":\"1.2.3\"}\r\n";
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
            var userAgent = Utilities.StripUnicodeCharactersFromString(Utilities.ConvertNA2Empty(message.Device?.UserAgent));
            var botResult = ProcessBotDetection(message, deviceInfo, userAgent);
            var ipResult = ProcessIP(message);

            GenerateFingerPrintSample(message);

            var url = Utilities.ConvertNA2Empty(Utilities.SubStringUrl(message.Device?.Url ?? string.Empty));

            return new RawTransactionModel
            {
                DeviceCode = deviceInfo.Tagging,
                InvalidDevice = string.IsNullOrWhiteSpace(message.Device?.Di) ? null : message.Device.Di,
                TransStatus = Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                SubscriberId = message.SubscriberId,
                TransTime = Convert.ToDateTime(message.TransDate.ToString(DATETIME_FORMAT, System.Globalization.CultureInfo.InvariantCulture)),
                PluginID = DEFAULT_PLUGIN_ID,
                URL = url,
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = userAgent,
                FlaggedCode = botResult.FlaggedCode,
                BotDetection = botResult.BotDetection,
                BotComponent = botResult.BotDModel?.Component ?? string.Empty,
                IP = ipResult.RealIP,
                IPId = ipResult.IpConverted?.IpNumber ?? 0,
                FakeIP = message.FakeIP?.Trim(),
                IsIPV6 = ipResult.IpConverted?.IsIpV6 ?? false,
                ScriptVersion = deviceInfo.Version,
                WebRTCIP = deviceInfo.Rtc?.Trim(),
                FPPatternCode01 = message.FPPatternCode01?.Trim(),
                FPPatternCode02 = message.FPPatternCode02?.Trim(),
                FingerPrint = message.FingerPrint
            };
        }

        private static void GenerateFingerPrintSample(MessageQueue message)
        {
            var fp = new APFingerprintModel
            {
                Audio = "{\"sampleRate\":44100,\"channelCount\":2,\"fingerprint\":\"audiofp123456\"}",
                Browser = "{\"userAgent\":\"Mozilla/5.0 (Windows NT 10.0; Win64; x64) Chrome/115.0.0.0 Safari/537.36\",\"language\":\"en-US\",\"languages\":[\"en-US\",\"en\"],\"doNotTrack\":\"1\",\"cookieEnabled\":true,\"javaEnabled\":false,\"timezone\":\"Asia/Ho_Chi_Minh\"}",
                Hardware = "{\"platform\":\"Win32\",\"architecture\":\"x64\",\"oscpu\":\"Windows NT 10.0; Win64; x64\",\"cpuClass\":\"x86\",\"hardwareConcurrency\":8,\"deviceMemory\":16}",
                Preferences = "{\"darkMode\":true,\"saveData\":false,\"reducedMotion\":false}",
                Graphics = "{\"canvas\":{\"fingerprint\":\"ab12cd34ef56\",\"status\":\"success\",\"width\":300,\"height\":150},\"webglBasics\":{\"vendor\":\"Google Inc.\",\"renderer\":\"ANGLE (NVIDIA GeForce GTX 1650 Direct3D11 vs_5_0 ps_5_0)\",\"version\":\"WebGL 1.0\",\"shadingLanguageVersion\":\"WebGL GLSL ES 1.0\"},\"fonts\":{\"installed\":[\"Arial\",\"Verdana\",\"Times New Roman\"]},\"screenResolution\":{\"width\":1920,\"height\":1080},\"colorDepth\":{\"value\":24}}"
            };

            fp.GenerateGroupHashCodes();

            message.FingerPrint = fp;
            message.FPPatternCode01 = "FPPatternCode01Sample";
            message.FPPatternCode02 = "FPPatternCode02Sample";
        }

        // Helper methods
        private void LogError(string message, Exception? exception = null)
        {
            var fullMessage = $"{appSettings.Environment} - {message}";
            if (exception != null)
                fullMessage += $"\n{exception}";
            _ = LogService.ErrorAsync(fullMessage);
        }

        // Result records
        private sealed record DeviceProcessingResult(DeviceInfoModel DeviceInfo, string? Url, string? UserAgent, string? OriginalDeviceCode);
        private sealed record BotDetectionResult(BotDModel BotDModel, string? FlaggedCode, string? BotDetection);
        private sealed record IpProcessingResult(string? RealIP, IpModel? IpConverted);
    }
}
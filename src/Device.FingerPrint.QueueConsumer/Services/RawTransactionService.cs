namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.Redis;
    using Device.FingerPrint.TransactionAPI.Model;
    using Fanex.Logging;
    using StackExchange.Redis;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public interface IRawTransactionService
    {
        RawTransactionModel TransformRawTransaction(MessageQueue message);

        void InsertOrUpdateEverTagging(string hashKey, IEnumerable<string> everTaggings, long expireSecond);

    }

    public class RawTransactionService : IRawTransactionService
    {
        private readonly IDeviceService deviceService;
        private readonly IBotDetectionService botDetectionService;
        private readonly IAppSettings appSettings;
        private readonly IDatabase everTaggingRedisDB;

        public RawTransactionService(
            IRedisMultiplexDbService redisMultiplexService,
            IDeviceService deviceService,
            IBotDetectionService botDetectionService,
            IAppSettings appSettings
        )
        {
            this.deviceService = deviceService;
            this.botDetectionService = botDetectionService;
            this.appSettings = appSettings;

            this.everTaggingRedisDB = redisMultiplexService.GetDatabase(appSettings.EverTaggingDBIndex);
        }

        public RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            try
            {
                var (deviceInfo, url, userAgent, invalidDevice) = ProcessDeviceInfo(message);
                var transBit = ProcessDeviceValidation(deviceInfo);
                var (botDModel, flaggedCode, botDetection) = ProcessBotDetection(message, deviceInfo, userAgent);
                var (realIP, ipConverted) = ProcessIPInfo(message);

                return CreateRawTransactionModel(message, deviceInfo, url, userAgent, invalidDevice, transBit, botDModel, flaggedCode, botDetection, realIP, ipConverted);
            }
            catch (Exception ex)
            {
                _ = LogService.ErrorAsync($"{appSettings.Environment} - TransformRawTransaction has error:\n {ex.Message}");
                return null;
            }
        }

        private (DeviceInfoModel deviceInfo, string url, string userAgent, string invalidDevice) ProcessDeviceInfo(MessageQueue message)
        {
            var deviceInfo = new DeviceInfoModel();
            var url = string.Empty;
            var userAgent = string.Empty;
            var invalidDevice = string.Empty;

            if (message.Device != null)
            {
                var deviceCode = message.Device.Di;
                deviceInfo = deviceService.GetDeviceInfo(deviceCode, message);
                url = message.Device.Url;
                userAgent = message.Device.UserAgent;
                invalidDevice = deviceCode; // Store the original device code for potential use
            }

            return (deviceInfo, url, userAgent, invalidDevice);
        }

        private BitArray ProcessDeviceValidation(DeviceInfoModel deviceInfo)
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

        private (string realIP, IpModel ipConverted) ProcessIPInfo(MessageQueue message)
        {
            var realIP = message.IP?.Trim();
            var ipConverted = realIP.Ip2Long();

            return (realIP, ipConverted);
        }


#pragma warning disable S107
        private RawTransactionModel CreateRawTransactionModel(
            MessageQueue message,
            DeviceInfoModel deviceInfo,
            string url,
            string userAgent,
            string invalidDevice,
            BitArray transBit,
            BotDModel botDModel,
            string flaggedCode,
            string botDetection,
            string realIP,
            IpModel ipConverted)
        {
            return new RawTransactionModel
            {
                DeviceCode = deviceInfo.DeviceNo,
                InvalidDevice = string.IsNullOrWhiteSpace(invalidDevice) ? null : invalidDevice,
                TransStatus = Core.Utilities.ConvertBitArray2Int(transBit),
                LoginName = message.Account,
                SubscriberName = message.Subscriber,
                SubscriberId = message.SubscriberId,
                TransTime = Convert.ToDateTime(message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff", System.Globalization.CultureInfo.InvariantCulture)),
                PluginID = 0,
                URL = string.IsNullOrWhiteSpace(url) ? Utilities.ConvertNA2Empty(Utilities.SubStringUrl(deviceInfo.HRef)) : Utilities.SubStringUrl(url),
                Action = message.Action,
                ActionResult = message.ActionResult,
                UserAgent = string.IsNullOrWhiteSpace(userAgent)
                    ? Utilities.ConvertNA2Empty(Utilities.StripUnicodeCharactersFromString(deviceInfo.UserAgent))
                    : Utilities.StripUnicodeCharactersFromString(userAgent),
                SessionKey = message.SessionKey,
                FlaggedCode = flaggedCode,
                BotDetection = botDetection,
                BotComponent = botDModel?.Component ?? string.Empty,
                IP = realIP,
                IPId = ipConverted.IpNumber,
                FakeIP = message.FakeIP?.Trim(),
                IsIPV6 = ipConverted.IsIpV6,
                ScriptVersion = deviceInfo.Version
            };
        }
#pragma warning restore S107

        public void InsertOrUpdateEverTagging(string hashKey, IEnumerable<string> everTaggings, long expireSecond)
        {
            try
            {
                foreach (var code in everTaggings)
                {
                    var expiry = TimeSpan.FromSeconds(expireSecond);

                    everTaggingRedisDB.StringSet(
                        key: $"{hashKey}:{code}",
                        value: DateTimeOffset.Now.ToString(),
                        expiry: expiry,
                        flags: CommandFlags.FireAndForget
                    );
                }
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }
    }
}

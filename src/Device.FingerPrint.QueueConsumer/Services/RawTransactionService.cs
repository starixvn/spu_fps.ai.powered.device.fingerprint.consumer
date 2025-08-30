namespace Device.FingerPrint.QueueConsumer.Services
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Features.Transaction.Services;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.Redis;
    using Fanex.Logging;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public interface IRawTransactionService
    {
        RawTransactionModel TransformRawTransaction(MessageQueue message);

        void InsertOrUpdateEverTagging(string hashKey, IEnumerable<string> everTaggings, long expireSecond);

        void InsertRawTransactionDetail(IEnumerable<RawTransactionModel> transactions, bool isTransOfDBExtra);

        void LogTransactions(IEnumerable<RawTransactionModel> transactions);

    }

    public class RawTransactionService : IRawTransactionService
    {
        private readonly IDeviceService deviceService;
        private readonly IBotDetectionService botDetectionService;
        private readonly ITransactionService transactionService;
        private readonly IAppSettings appSettings;
        private readonly IDatabase everTaggingRedisDB;

        public RawTransactionService(
            IRedisMultiplexDbService redisMultiplexService,
            IDeviceService deviceService,
            IBotDetectionService botDetectionService,
            ITransactionService transactionService,
            IAppSettings appSettings
        )
        {
            this.deviceService = deviceService;
            this.botDetectionService = botDetectionService;
            this.transactionService = transactionService;
            this.appSettings = appSettings;

            this.everTaggingRedisDB = redisMultiplexService.GetDatabase(appSettings.EverTaggingDBIndex);
        }

        public RawTransactionModel TransformRawTransaction(MessageQueue message)
        {
            try
            {
                var invalidDevice = string.Empty;
                var deviceCode = string.Empty;
                var url = string.Empty;
                var userAgent = string.Empty;
                var deviceInfo = new DeviceInfoModel();

                if (message.Device != null)
                {
                    deviceCode = message.Device.Di;
                    deviceInfo = deviceService.GetDeviceInfo(deviceCode, message);
                    url = message.Device.Url;
                    userAgent = message.Device.UserAgent;
                }

                var transBit = new BitArray(16);
                if (string.IsNullOrWhiteSpace(deviceInfo.DeviceNo) || deviceInfo.DeviceNo == "N/A")
                {
                    deviceInfo.DeviceNo = null;
                    transBit.Set((int)TransStatus.DeviceCodeIsEmpty, true);
                }
                else
                {
                    if (!Utilities.IsGuidDeviceId(deviceInfo.DeviceNo) || deviceInfo.InvalidDeviceId)
                    {
                        invalidDevice = deviceCode;
                        deviceInfo.DeviceNo = null;
                        transBit.Set((int)TransStatus.InvalidDI, true);
                    }
                }

                var botDModel = botDetectionService.TransformBotdModel(message);

                var flaggedCode = botDetectionService.GetFlaggedCode(botDModel, deviceInfo.DeviceNo, userAgent);
                var botDetection = botDModel?.IsVer3 == true ? botDModel?.Detection : botDetectionService.GetBotDetectionTrue(botDModel);

                var realIP = message.IP?.Trim();
                var ipConverted = realIP.Ip2Long();

                var rawTransModel = new RawTransactionModel
                {
                    DeviceCode = deviceInfo.DeviceNo,
                    InvalidDevice = string.IsNullOrWhiteSpace(invalidDevice) ? null : invalidDevice,
                    TransStatus = Core.Utilities.ConvertBitArray2Int(transBit),
                    LoginName = message.Account,
                    SubscriberName = message.Subscriber,
                    SubscriberId = message.SubscriberId,
                    TransTime = Convert.ToDateTime(
                        message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff")
                    ),
                    PluginID = 0,
                    URL = string.IsNullOrWhiteSpace(url)
                        ? Utilities.ConvertNA2Empty(
                            Utilities.SubStringUrl(deviceInfo.HRef)
                        )
                        : Utilities.SubStringUrl(url),
                    Action = message.Action,
                    ActionResult = message.ActionResult,
                    UserAgent = string.IsNullOrWhiteSpace(userAgent)
                        ? Utilities.ConvertNA2Empty(
                            Utilities.StripUnicodeCharactersFromString(deviceInfo.UserAgent)
                        )
                        : Utilities.StripUnicodeCharactersFromString(userAgent),
                    FingerprintCode = deviceInfo.DeviceFPCode != "N/A"
                        ? deviceInfo.DeviceFPCode
                        : null,
                    FingerprintMoreInfo = deviceInfo.DeviceFPCode != "N/A"
                        ? Utilities.EncodeCapturedData(deviceInfo)
                        : null,
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

                return rawTransModel;

            }
            catch (Exception ex)
            {
                _ = LogService.ErrorAsync($"{appSettings.Environment} - TransformRawTransaction has error:\n {ex.Message}");

                return null;
            }
        }

        public void InsertOrUpdateEverTagging(string hashKey, IEnumerable<string> everTaggings, long expireSecond)
        {
            try
            {
                var expiry = TimeSpan.FromSeconds(expireSecond);
                var locale = System.Globalization.CultureInfo.InvariantCulture;
                foreach (var code in everTaggings)
                {
                    var key = $"{hashKey}:{code}";
                    var value = DateTimeOffset.Now.ToString(locale);
                    everTaggingRedisDB.StringSet(
                        key: key,
                        value: value,
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

        public void InsertRawTransactionDetail(IEnumerable<RawTransactionModel> transactions, bool isTransOfDBExtra)
        {
            Parallel.Invoke(
                () => InsertFPFingerPrint(transactions, isTransOfDBExtra),
                () => InsertFPUserAgent(transactions, isTransOfDBExtra)
            );
        }

        private void InsertFPUserAgent(IEnumerable<RawTransactionModel> transactions, bool isTransOfDBExtra)
        {
            try
            {
                var userAgentInfos = GetValidInfo(transactions, t => t.UserAgentInfo);
                if (userAgentInfos != null && userAgentInfos.Any())
                {
                    var userAgentJson = JsonConvert.SerializeObject(userAgentInfos);
                    transactionService.InsertFPUserAgent(userAgentJson, isTransOfDBExtra);
                }
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }
        

        private void InsertFPFingerPrint(IEnumerable<RawTransactionModel> transactions, bool isTransOfDBExtra)
        {
            try
            {
                var fingerprintAttribute = GetValidInfo(transactions, t => t.FingerprintAttribute);
                if (fingerprintAttribute != null && fingerprintAttribute.Any())
                {
                    var fingerPrintJson = JsonConvert.SerializeObject(fingerprintAttribute);
                    transactionService.InsertFPFingerPrint(fingerPrintJson, isTransOfDBExtra);
                }
            }
            catch (Exception ex)
            {
                _ = QueueConsumer.Utilities.LogErrorAsync(ex);
            }
        }

        private static IEnumerable<TResult> GetValidInfo<TResult>(IEnumerable<RawTransactionModel> transactions, Func<RawTransactionModel, TResult> selector)
        {
            return transactions
                .Select(selector)
                .Where(result => !Equals(result, default(TResult)))
                .Distinct();
        }
       
        public void LogTransactions(IEnumerable<RawTransactionModel> transactions)
        {
            // ...existing code...
        }     

    }
}

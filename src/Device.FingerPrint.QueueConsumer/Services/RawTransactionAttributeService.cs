using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Encoders;
using Device.FingerPrint.Core.Services;
using Device.FingerPrint.Features.LogInfo.Model;
using Device.FingerPrint.Features.LogInfo.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.Model.DTO;
using Device.FingerPrint.Model.Enums;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.Redis;
using DeviceDetectorNET;
using Fanex.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Device.FingerPrint.QueueConsumer.Services
{
    public interface IRawTransactionAttributeService
    {
        RawTransactionModel GetTransactionAttribute(RawTransactionModel model);
    }

    public class RawTransactionAttributeService : IRawTransactionAttributeService
    {
        private readonly ICrypto crypto;
        private readonly IRedisMultiplexDbService redisMultiplexService;
        private readonly ILogInfoServices logInfoServices;
        private readonly IAppSettings appSettings;
        private readonly IDatabase everTaggingRedisDB;

        private const string base64Key = "wFKfVDGZqRbyYCBi6IveMaQAh1ntWcP8ujT/U7r2xXs+J0L=4d3zEHSo5glN9kOpm";
        private const string _realTaggingPrefix = "RealTagging:";
        private readonly List<string> fieldKeys = [];
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromMilliseconds(500);

        public RawTransactionAttributeService(
            ICrypto crypto,
            IRedisMultiplexDbService redisMultiplexService,
            ILogInfoServices logInfoServices,
            IAppSettings appSettings)
        {
            this.crypto = crypto;
            this.redisMultiplexService = redisMultiplexService;
            this.logInfoServices = logInfoServices;
            this.appSettings = appSettings;

            everTaggingRedisDB = redisMultiplexService.GetDatabase(appSettings.EverTaggingDBIndex);

            fieldKeys = new List<string>
                            {
                                RedisString.GetRedisKeyName(Model.RedisKey.FingerPrintEncode),
                                RedisString.GetRedisKeyName(Model.RedisKey.Header)
                            };
        }

        public RawTransactionModel GetTransactionAttribute(RawTransactionModel model)
        {
            try
            {
                if (model != null)
                {
                    ProcessModel(model);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogInfo(ex);
            }

            return model;
        }

        private void ProcessModel(RawTransactionModel model)
        {
            CheckScriptVersion(model);

            model.RawUserAgent = model.UserAgent;
            model.UserAgentInfo = GetUserAgentInfo(model.UserAgent);
            model.FPUserAgentCode = crypto.GetMD5Hash(model.UserAgent);

            ProcessSessionKey(model);
        }

#pragma warning disable S1541
        private void ProcessSessionKey(RawTransactionModel model)
        {
            if (string.IsNullOrEmpty(model.SessionKey))
            {
                if (model.SessionKey == null)
                {
                    _ = LogConsumerErrorAsync(model, LogType.SessionKeyNull);
                }
                return;
            }
            else
            {
                // Continue processing when SessionKey is not null or empty
            }

            var isLogged = false;

            if (!model.SessionKey.Contains(':'))
            {
                _ = LogConsumerErrorAsync(model, LogType.SessionKeyColon);
                isLogged = true;
            }

            var sessionKey = Core.Utilities.ParseSessionKey(model.SessionKey, appSettings.RedisDatabase);
            if (sessionKey != null)
            {
                ProcessValidSessionKey(model, sessionKey);
            }
            else if (!isLogged)
            {
                _ = LogConsumerErrorAsync(model, LogType.SessionKeyParse);
            }
            else
            {
                // No action needed when sessionKey is null and already logged
            }
        }
#pragma warning restore S1541

        private void ProcessValidSessionKey(RawTransactionModel model, SessionKeyModel sessionKey)
        {
            var redisValues = redisMultiplexService.HashGetWithDbIndex(sessionKey.RedisDbIndex, sessionKey.SessionKey, fieldKeys);

            var fingerPrintEncode = redisValues[0];
            var header = redisValues[1];

            var fingerPrintModel = GetFingerPrintModel(fingerPrintEncode);
            var headerModel = GetHeaderModel(header);
            UpdateIPInfo(model, headerModel);

            model.FingerprintAttribute = TransformFingerprintAttributes(model, fingerPrintModel, headerModel);

            HandleTaggingProcess(model, sessionKey.SessionKey);
        }

#pragma warning disable S1541
        private void CheckScriptVersion(RawTransactionModel model)
        {
            var curScriptVersion = appSettings.GetSection<string>("Consumer:ScriptVersion");
            var compareScriptVersion = CompareVersionsWithTryParse(model.ScriptVersion, curScriptVersion);

            if (compareScriptVersion == 0)
            {
                _ = LogConsumerErrorAsync(model, LogType.ScriptVersionOld);
            }
            else if (compareScriptVersion == -1)
            {
                const int maxLength = 10;
                if (!string.IsNullOrEmpty(model.ScriptVersion) && model.ScriptVersion.Length > maxLength)
                {
                    model.ScriptVersion = model.ScriptVersion.Substring(0, maxLength);
                }

                _ = LogConsumerErrorAsync(model, LogType.ScriptVersionParse);
            }
            else
            {
                // Script version is valid - no action needed
            }
        }
#pragma warning restore S1541

        private static int CompareVersionsWithTryParse(string versionA, string versionB)
        {
            if (Version.TryParse(versionA, out var verA) && Version.TryParse(versionB, out var verB))
            {
                if (verA.Major != verB.Major)
                {
                    return 0;
                }

                return verA < verB ? 0 : 1;
            }

            return -1;
        }

        private void HandleTaggingProcess(RawTransactionModel model, string sessionKey)
        {
            var realTagging = everTaggingRedisDB.StringGet(_realTaggingPrefix + sessionKey);
            model.FPTagging = GetTaggingValue(realTagging, sessionKey);

            if (string.IsNullOrEmpty(model.FPTagging))
            {
                _ = LogConsumerErrorAsync(model, LogType.TaggingEncrypt);
            }
            else if (model.FPTagging.Length != 32)
            {
                _ = LogConsumerErrorAsync(model, LogType.TaggingLength);
            }
            else
            {
                // FPTagging is valid - no action needed
            }

            if (!string.IsNullOrEmpty(realTagging))
            {
                _ = everTaggingRedisDB.KeyDeleteAsync(_realTaggingPrefix + sessionKey);
            }
            else
            {
                // No action needed when realTagging is null or empty
            }
        }

        private FingerprintModel GetFingerPrintModel(string fingerPrintEncode)
        {
            if (!string.IsNullOrEmpty(fingerPrintEncode))
            {
                try
                {
                    var decodeData = Base64.Decode(fingerPrintEncode, base64Key);
                    if (Utilities.IsValidJson(decodeData))
                    {
                        return JsonConvert.DeserializeObject<FingerprintModel>(decodeData);
                    }
                }
                catch (Exception ex)
                {
                    _ = LogService.InfoAsync($"{appSettings.Environment} - Deserialize fingerPrint has error: {ex.Message}\n{fingerPrintEncode}");
                }
            }

            return new FingerprintModel();
        }

        private HeaderModel GetHeaderModel(string header)
        {
            if (!string.IsNullOrEmpty(header) && Utilities.IsValidJson(header))
            {
                try
                {
                    return JsonConvert.DeserializeObject<HeaderModel>(header);
                }
                catch (Exception ex)
                {
                    _ = LogService.InfoAsync($"{appSettings.Environment} - Deserialize header has error:\nheader: {header}\n {ex.Message}");
                }
            }

            return new HeaderModel();
        }

#pragma warning disable S1541
        private static void UpdateIPInfo(RawTransactionModel model, HeaderModel headerModel)
        {
            if (model == null || headerModel == null)
            {
                return;
            }

            var realIP = headerModel.RealIP?.Trim();

            if (!string.IsNullOrEmpty(realIP))
            {
                var ipConverted = realIP.Ip2Long();

                model.IP = realIP;
                model.IPId = ipConverted?.IpNumber ?? 0;
                model.IsIPV6 = ipConverted?.IsIpV6 ?? false;

                if (!string.IsNullOrEmpty(headerModel.FakeIP))
                {
                    model.FakeIP = headerModel.FakeIP?.Trim();
                }
            }
        }
#pragma warning restore S1541

        private InsertFPFingerprintEntity TransformFingerprintAttributes(RawTransactionModel rawTransModel, FingerprintModel fpModel, HeaderModel headerModel)
        {
            var result = new InsertFPFingerprintEntity();

            if (!ValidateInputs(rawTransModel, fpModel))
            {
                return result;
            }

            var attributesDictionary = DeserializeToDictionary(fpModel.Attributes);
            if (!ValidateAttributesDictionary(attributesDictionary))
            {
                return result;
            }

            ProcessFingerprintAttributes(rawTransModel, attributesDictionary, headerModel, fpModel.Platform, result);

            return result;
        }

        private static bool ValidateInputs(RawTransactionModel rawTransModel, FingerprintModel fpModel)
        {
            return rawTransModel != null && fpModel != null && !string.IsNullOrEmpty(fpModel.Attributes);
        }

        private static bool ValidateAttributesDictionary(Dictionary<string, string> attributesDictionary)
        {
            return attributesDictionary != null && attributesDictionary.Any();
        }

        private void ProcessFingerprintAttributes(
            RawTransactionModel rawTransModel,
            Dictionary<string, string> attributesDictionary,
            HeaderModel headerModel,
            string platform,
            InsertFPFingerprintEntity result)
        {
            UpdateIsIncognitoMode(rawTransModel, attributesDictionary, rawTransModel.UserAgentInfo?.ClientName);

            var fingerPrintAttribute = TransformFPFingerPrintInfo(attributesDictionary);
            if (fingerPrintAttribute == null)
            {
                return;
            }

            UpdateFPFingerPrintInfo(result, fingerPrintAttribute);

            var oldFingerPrintAttribute = TransformFPOldFingerprintInfo(attributesDictionary, fingerPrintAttribute, rawTransModel.UserAgentInfo);
            UpdateFPOldFingerprintInfo(result, oldFingerPrintAttribute);

            var fingerprintExtra = TransformFingerprintAttributeExtra(headerModel, rawTransModel, oldFingerPrintAttribute, platform);
            UpdateFPFingerprintExtra(result, fingerprintExtra);

            UpdateRawTransactionModel(rawTransModel, result);
        }

        private static void UpdateRawTransactionModel(RawTransactionModel rawTransModel, InsertFPFingerprintEntity result)
        {
            rawTransModel.FPFingerPrintCode = result.FPFingerPrintCode;
            rawTransModel.FPFingerPrintExtraCode = result.FPFingerPrintExtraCode;
            rawTransModel.FPFingerPrintOldCode = result.FPFingerPrintOldCode;
        }

        private static void UpdateIsIncognitoMode(RawTransactionModel rawTransModel, Dictionary<string, string> attributesDictionary, string browserName)
        {
            if (!string.IsNullOrEmpty(browserName) && attributesDictionary.TryGetValue("isIncognitoMode", out var value) && bool.TryParse(value, out var result))
            {
                rawTransModel.IsIncognitoMode = result;
                if (IgnoreDetectPrivateMode(browserName))
                {
                    rawTransModel.IsIncognitoMode = false;
                }
            }
            else
            {
                rawTransModel.IsIncognitoMode = null;
            }
        }

        private static bool IgnoreDetectPrivateMode(string browserName)
        {
            const string _UCBROWSER = "uc browser";
            const string _MIBROWSER = "miui browser";

            if (browserName.ToLowerInvariant().Contains(_UCBROWSER))
            {
                return true;  //no need to detect incognito mode
            }

            if (browserName.ToLowerInvariant().Contains(_MIBROWSER))
            {
                return true;  //skip to detect incognito mode
            }

            return false;
        }

        private UserAgentInfoModel GetUserAgentInfo(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return new UserAgentInfoModel();
            }

            var deviceDetector = new DeviceDetector(userAgent);
            deviceDetector.SkipBotDetection();
            deviceDetector.Parse();

            var userAgentInfo = new UserAgentInfoModel { UserAgent = userAgent };

            ProcessClientInfo(deviceDetector, userAgentInfo);
            ProcessOSInfo(deviceDetector, userAgentInfo);
            ProcessDeviceInfo(deviceDetector, userAgentInfo);

            userAgentInfo.FPUserAgentCode = crypto.GetMD5Hash(userAgent);

            return userAgentInfo;
        }

        private static void ProcessClientInfo(DeviceDetector deviceDetector, UserAgentInfoModel userAgentInfo)
        {
            var clientInfo = deviceDetector.GetClient();

            if (clientInfo != null && clientInfo.Success)
            {
                userAgentInfo.ClientType = clientInfo.Match.Type;
                userAgentInfo.ClientName = clientInfo.Match.Name;
                userAgentInfo.ClientVersion = clientInfo.Match.Version;
            }
        }

        private static void ProcessOSInfo(DeviceDetector deviceDetector, UserAgentInfoModel userAgentInfo)
        {
            var osInfo = deviceDetector.GetOs();

            if (osInfo != null && osInfo.Success)
            {
                userAgentInfo.OSName = osInfo.Match.Name;
                userAgentInfo.OSShortName = osInfo.Match.ShortName;
                userAgentInfo.OSVersion = osInfo.Match.Version;
                userAgentInfo.OSPlatform = osInfo.Match.Platform;
            }
        }

        private static void ProcessDeviceInfo(DeviceDetector deviceDetector, UserAgentInfoModel userAgentInfo)
        {
            userAgentInfo.Device = deviceDetector.GetDeviceName();
            userAgentInfo.Brand = deviceDetector.GetBrandName();
            userAgentInfo.Model = deviceDetector.GetModel();
            userAgentInfo.DeviceType = GetDeviceType(deviceDetector);
        }

        private static Dictionary<string, string> DeserializeToDictionary(string attributesJson)
        {
            if (string.IsNullOrEmpty(attributesJson))
            {
                return new Dictionary<string, string>();
            }

            var deserializedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(attributesJson);
            return deserializedData.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value == null ? null : JsonConvert.SerializeObject(kvp.Value));
        }


        private static FPFingerprintAttributeModel TransformFPFingerPrintInfo(Dictionary<string, string> attributesDictionary)
        {
            return JsonConvert.DeserializeObject<FPFingerprintAttributeModel>(JsonConvert.SerializeObject(attributesDictionary));
        }

        private void UpdateFPFingerPrintInfo(InsertFPFingerprintEntity result, FPFingerprintAttributeModel fingerPrintAttribute)
        {
            if (HasData(fingerPrintAttribute))
            {
                var newFingerprintModel = new FPNewFingerprintModel(fingerPrintAttribute, crypto);
                result.FPFingerPrintCode = newFingerprintModel.Code;
                result.FPFingerPrintAttribute = newFingerprintModel.AttributeModel;
            }
        }

        private static FPOldFingerprintAttributeModel TransformFPOldFingerprintInfo(
            Dictionary<string, string> attributesDictionary,
            FPFingerprintAttributeModel fingerPrintAttribute,
            UserAgentInfoModel userAgentInfo)
        {
            var oldFingerPrintAttribute = JsonConvert.DeserializeObject<FPOldFingerprintAttributeModel>(JsonConvert.SerializeObject(attributesDictionary));
            oldFingerPrintAttribute.SuperCookies = GetSuperCookies(fingerPrintAttribute);

            if (userAgentInfo != null)
            {
                oldFingerPrintAttribute.UserAgent = userAgentInfo.UserAgent;
                oldFingerPrintAttribute.Browser = userAgentInfo.ClientName + " " + userAgentInfo.ClientVersion;
                oldFingerPrintAttribute.DeviceType = userAgentInfo.DeviceType;
            }

            return oldFingerPrintAttribute;
        }

        private void UpdateFPOldFingerprintInfo(InsertFPFingerprintEntity result, FPOldFingerprintAttributeModel oldFingerPrintAttribute)
        {
            if (HasData(oldFingerPrintAttribute))
            {
                if (!string.IsNullOrEmpty(oldFingerPrintAttribute.UserAgent))
                {
                    oldFingerPrintAttribute.UserAgent = CleanUserAgentFuzzy(oldFingerPrintAttribute.UserAgent);
                }
                var oldFingerprintModel = new FPOldFingerprintModel(oldFingerPrintAttribute, crypto);
                result.FPFingerPrintOldCode = oldFingerprintModel.Code;
            }
        }

        private void UpdateFPFingerprintExtra(InsertFPFingerprintEntity result, FPFingerprintAttributeExtraModel fingerprintExtra)
        {
            if (HasData(fingerprintExtra))
            {
                if (!string.IsNullOrEmpty(fingerprintExtra.UserAgent))
                {
                    fingerprintExtra.UserAgent = CleanUserAgentFuzzy(fingerprintExtra.UserAgent);
                }
                var fingerprintExtraModel = new FPFingerprintExtraModel(fingerprintExtra, crypto);
                result.FPFingerPrintExtraCode = fingerprintExtraModel.Code;
                result.FPFingerPrintExtraAttribute = fingerprintExtraModel.AttributeModel;
            }
        }

        private static FPFingerprintAttributeExtraModel TransformFingerprintAttributeExtra(
            HeaderModel headerModel,
            RawTransactionModel rawTransModel,
            FPOldFingerprintAttributeModel oldFingerPrintAttribute,
            string platform)
        {
            var fingerprintAttributeExtra = new FPFingerprintAttributeExtraModel
            {
                Platform = platform,
                Accept = headerModel?.Accept,
                Encoding = headerModel?.Encoding,
                Headers = headerModel?.Headers,
                UserAgent = rawTransModel.UserAgentInfo?.UserAgent,
                BrowserName = rawTransModel.UserAgentInfo?.ClientName,
                BrowserVersion = rawTransModel.UserAgentInfo?.ClientVersion,
                OSName = rawTransModel.UserAgentInfo?.OSName,
                Navigator = oldFingerPrintAttribute.Navigator,
                TimezoneOffset = oldFingerPrintAttribute.TimezoneOffset,
                DeviceType = oldFingerPrintAttribute.DeviceType
            };

            return fingerprintAttributeExtra;
        }

        private static bool HasData<T>(T model)
        {
            var properties = model.GetType().GetProperties();

            foreach (var property in properties)
            {
                var value = property.GetValue(model);

                if (value != null && !(value is string str && string.IsNullOrEmpty(str)))
                {
                    return true;
                }
            }

            return false;
        }

        private static int GetDeviceType(DeviceDetector deviceDetector)
        {
            const int _OTHERTYPE = 0;
            const int _DESTOPTYPE = 1;
            const int _TABLETTYPE = 2;
            const int _MOBILETYPE = 3;

            if (deviceDetector.IsDesktop())
            {
                return _DESTOPTYPE;
            }

            if (deviceDetector.IsTablet())
            {
                return _TABLETTYPE;
            }

            if (deviceDetector.IsMobile())
            {
                return _MOBILETYPE;
            }

            return _OTHERTYPE;
        }

        private static string GetSuperCookies(FPFingerprintAttributeModel fingerprintAttributeModel)
        {
            var localStorage = fingerprintAttributeModel?.LocalStorage == "true";
            var sessionStorage = fingerprintAttributeModel?.SessionStorage == "true";
            var openDatabase = fingerprintAttributeModel?.OpenDatabase == "true";
            var indexedDb = fingerprintAttributeModel?.IndexedDB == "true";

            return $"{localStorage}|{sessionStorage}|{openDatabase}|{indexedDb}";
        }

        // Method intentionally left empty - duplicate functionality removed

        private static string GetTaggingValue(string realTagging, string sessionKey)
        {
            if (!string.IsNullOrEmpty(realTagging) && IsHexString(realTagging))
            {
                try
                {
                    var decryptedStr = DeviceEncoder.Decrypt(realTagging);
                    if (CheckGuid(decryptedStr))
                    {
                        return decryptedStr;
                    }
                }
                catch
                {
                    // Exception intentionally ignored - decryption failed
                }
            }

            if (!string.IsNullOrEmpty(sessionKey) && IsHexString(sessionKey))
            {
                try
                {
                    var decryptedStr1 = DeviceEncoder.Decrypt(sessionKey);
                    if (CheckGuid(decryptedStr1))
                    {
                        return decryptedStr1;
                    }
                }
                catch
                {
                    // Exception intentionally ignored - decryption failed
                }
            }

            return string.Empty;
        }

        private static bool IsHexString(string hex)
        {
            return Regex.IsMatch(hex,
                                @"\A\b[0-9a-fA-F]+\b\Z",
                                RegexOptions.None,
                                RegexTimeout // set timeout
                            );
        }

        private static bool CheckGuid(string decryptedText)
        {
            if (decryptedText.Length != 32)
            {
                return false;
            }

            return Guid.TryParse(decryptedText, out _);
        }

        private static string CleanUserAgentFuzzy(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return userAgent;
            }

            var trackingPatterns = new List<Regex>
            {
                new(@"\s+UUID\/[a-zA-Z0-9-]+", RegexOptions.Compiled, RegexTimeout),
                new(@"\s+HTTP_BB_FORWARDED\/[^\s]+", RegexOptions.Compiled, RegexTimeout),
                new(@"\s+FalconTag\/[^\s]+", RegexOptions.Compiled, RegexTimeout),
                new(@"\s+SessionId\/[a-zA-Z0-9]+", RegexOptions.Compiled, RegexTimeout)
            };

            var cleanedUA = userAgent;

            foreach (var pattern in trackingPatterns)
            {
                cleanedUA = pattern.Replace(cleanedUA, string.Empty);
            }

            cleanedUA = Regex.Replace(cleanedUA,
                                    @"\s*\[[^\]]*(?:\]|$)",
                                    string.Empty,
                                    RegexOptions.None,
                                    TimeSpan.FromSeconds(1) // timeout
                                );

            return Regex.Replace(cleanedUA,
                                @"\s{2,}",
                                " ",
                                RegexOptions.None,
                                TimeSpan.FromSeconds(1) // timeout
                            ).Trim();
        }

        #region Log Info
        private async Task LogConsumerErrorAsync(RawTransactionModel model, LogType logType)
        {
            try
            {
                if (appSettings.GetValue<bool>("Consumer:LogInfo"))
                {
                    var logInfoModel = TransformToLogInfoModel(model, (int)logType);
                    await logInfoServices.InsertDCSConsumerLogAsync(logInfoModel);
                }
            }
            catch (Exception ex)
            {
                Utilities.LogInfo(ex);
            }
        }

        private LogInfoModel TransformToLogInfoModel(RawTransactionModel model, int logType)
        {
            var sessionKey = model.SessionKey ?? string.Empty;
            var tagging = model.FPTagging ?? string.Empty;

            return new LogInfoModel
            {
                LogType = logType,
                Environment = appSettings.Environment,
                ScriptVersion = model.ScriptVersion,
                SessionKey = sessionKey,
                Tagging = tagging,
                LoginName = model.LoginName,
                SubscriberName = model.SubscriberName,
                TransTime = model.TransTime,
                URL = model.URL,
                UserAgent = model.UserAgent
            };
        }

        #endregion    
    }

}

using Microsoft.Extensions.Configuration;
using System;
using System.ComponentModel;

namespace Device.FingerPrint.Core
{
    public interface IAppSettings
    {
        string ApiUrl { get; }

        string PortalApiUrl { get; }

        string CryptoPassword { get; }

        string CryptoIV { get; }

        string CryptoSalt { get; }

        string SPName { get; }

        string SentryDsn { get; }

        string Environment { get; }

        int RedisDatabase { get; }

        long RedisExpired { get; }

        string JWTPassword { get; }

        string JWT { get; }

        string JWTIssue { get; }

        string JWTAudience { get; }

        bool IsExtraSub { get; }

        int MaxMonthsGetAssociationByDevice { get; }

        long EverTaggingExpireSecond { get; }

        int EverTaggingDBIndex { get; }

        long ApiEverTaggingCacheMaxAge { get; }

        bool IsDisableJsChallenge { get; }

        bool IsVaultEnable { get; }

        IConfiguration Configuration { get; }

        T GetValue<T>(string key);

        T GetSection<T>(string key);
    }

    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration configuration;

        public const string DeviceIdEncryptKey = "&％#@?,:*!~^.";

        private readonly string cdnApiUrl;
        private readonly string portalApiUrl;
        private readonly string apiUrl;
        private readonly string spName;
        private readonly string cryptoPassword;
        private readonly string sentryDsn;
        private readonly string environment;

        private readonly string cryptoIV;
        private readonly string cryptoSalt;

        private readonly long redisExpired;
        private readonly int redisDatabase;

        private readonly string jWTPassword;

        private readonly string jWTIssue;

        private readonly string jWTAudience;

        private readonly string jWT;

        private readonly bool isExtraSub;

        private readonly int maxMonthsGetAssociationByDevice;

        private readonly long everTaggingExpireSecond;

        private readonly int everTaggingDBIndex;

        private readonly long apiEverTaggingCacheMaxAge;

        private readonly bool isDisableJsChallenge;

        private readonly bool isVaultEnable;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            // Initialize all settings in constructor to avoid readonly field assignment issues
            var (apiUrlVal, cdnApiUrlVal, portalApiUrlVal, spNameVal, environmentVal, sentryDsnVal) = GetApiSettings();
            var (cryptoPasswordVal, cryptoIVVal, cryptoSaltVal) = GetCryptoSettings();
            var (redisDatabaseVal, redisExpiredVal, everTaggingExpireSecondVal, everTaggingDBIndexVal, apiEverTaggingCacheMaxAgeVal) = GetRedisSettings();
            var (jWTPasswordVal, jWTIssueVal, jWTAudienceVal, jWTVal) = GetJWTSettings();
            var (isExtraSubVal, maxMonthsGetAssociationByDeviceVal, isDisableJsChallengeVal) = GetConsumerSettings();
            var isVaultEnableVal = GetVaultSettings();

            // Assign to readonly fields
            apiUrl = apiUrlVal;
            cdnApiUrl = cdnApiUrlVal;
            portalApiUrl = portalApiUrlVal;
            spName = spNameVal;
            environment = environmentVal;
            sentryDsn = sentryDsnVal;
            cryptoPassword = cryptoPasswordVal;
            cryptoIV = cryptoIVVal;
            cryptoSalt = cryptoSaltVal;
            redisDatabase = redisDatabaseVal;
            redisExpired = redisExpiredVal;
            everTaggingExpireSecond = everTaggingExpireSecondVal;
            everTaggingDBIndex = everTaggingDBIndexVal;
            apiEverTaggingCacheMaxAge = apiEverTaggingCacheMaxAgeVal;
            jWTPassword = jWTPasswordVal;
            jWTIssue = jWTIssueVal;
            jWTAudience = jWTAudienceVal;
            jWT = jWTVal;
            isExtraSub = isExtraSubVal;
            maxMonthsGetAssociationByDevice = maxMonthsGetAssociationByDeviceVal;
            isDisableJsChallenge = isDisableJsChallengeVal;
            isVaultEnable = isVaultEnableVal;
        }

        private (string apiUrl, string cdnApiUrl, string portalApiUrl, string spName, string environment, string sentryDsn) GetApiSettings()
        {
            return (
                GetValue<string>("AppConfig:ApiUrl") ?? string.Empty,
                GetValue<string>("AppConfig:CDNApiUrl") ?? string.Empty,
                GetValue<string>("AppConfig:PortalApiUrl") ?? string.Empty,
                GetValue<string>("StoredProcedurePath") ?? "App_Data",
                GetValue<string>("AppConfig:Environment") ?? string.Empty,
                GetValue<string>("Sentry:Dsn") ?? string.Empty
            );
        }

        private (string cryptoPassword, string cryptoIV, string cryptoSalt) GetCryptoSettings()
        {
            return (
                GetValue<string>("AppConfig:Crypto:Password") ?? string.Empty,
                GetValue<string>("AppConfig:Crypto:IV") ?? string.Empty,
                GetValue<string>("AppConfig:Crypto:Salt") ?? string.Empty
            );
        }

        private (int redisDatabase, int redisExpired, long everTaggingExpireSecond, int everTaggingDBIndex, long apiEverTaggingCacheMaxAge) GetRedisSettings()
        {
            var redisDatabaseValue = GetValue<string>("Redis:Database");
            var redisExpiredValue = GetValue<string>("Redis:Expired");
            var everTaggingExpireValue = GetValue<string>("Redis:EverTaggingExpireSecond");
            var everTaggingDbIndexValue = GetValue<string>("Redis:EverTaggingDbIndex");
            var apiEverTaggingCacheValue = GetValue<string>("ApiEverTaggingCacheMaxAge");

            return (
                Convert.ToInt32(string.IsNullOrEmpty(redisDatabaseValue) ? "0" : redisDatabaseValue),
                Convert.ToInt32(string.IsNullOrEmpty(redisExpiredValue) ? "3600" : redisExpiredValue),
                Convert.ToInt64(string.IsNullOrEmpty(everTaggingExpireValue) ? "3600" : everTaggingExpireValue),
                Convert.ToInt32(string.IsNullOrEmpty(everTaggingDbIndexValue) ? "0" : everTaggingDbIndexValue),
                Convert.ToInt64(string.IsNullOrEmpty(apiEverTaggingCacheValue) ? "86400" : apiEverTaggingCacheValue)
            );
        }

        private (string jWTPassword, string jWTIssue, string jWTAudience, string jWT) GetJWTSettings()
        {
            return (
                GetValue<string>("AppConfig:JWT:Password") ?? "!QAZxdr567890",
                GetValue<string>("AppConfig:JWT:Issue") ?? string.Empty,
                GetValue<string>("AppConfig:JWT:Audience") ?? string.Empty,
                GetValue<string>("AppConfig:JWT") ?? string.Empty
            );
        }

        private (bool isExtraSub, int maxMonthsGetAssociationByDevice, bool isDisableJsChallenge) GetConsumerSettings()
        {
            var isExtraSubValue = GetValue<string>("Consumer:IsExtraSub");
            var maxMonthsValue = GetValue<string>("API:MaxMonthsGetAssociationByDevice");
            var isDisableJsChallengeValue = GetValue<string>("Consumer:IsDisableJsChallenge");

            return (
                !string.IsNullOrEmpty(isExtraSubValue) && Convert.ToBoolean(isExtraSubValue),
                Convert.ToInt32(string.IsNullOrEmpty(maxMonthsValue) ? "0" : maxMonthsValue),
                !string.IsNullOrEmpty(isDisableJsChallengeValue) && Convert.ToBoolean(isDisableJsChallengeValue)
            );
        }

        private bool GetVaultSettings()
        {
            var isVaultEnableValue = GetValue<string>("VaultOptions:Enable");
            return !string.IsNullOrEmpty(isVaultEnableValue) && Convert.ToBoolean(isVaultEnableValue);
        }

        public string ApiUrl => apiUrl;

        public string CryptoPassword => cryptoPassword;

        public string SPName => spName;

        public string SentryDsn => sentryDsn;

        public string Environment => environment;

        public string CryptoIV => cryptoIV;

        public string CryptoSalt => cryptoSalt;

        public string CDNApiUrl => cdnApiUrl;

        public string PortalApiUrl => portalApiUrl;

        public int RedisDatabase => redisDatabase;

        public long RedisExpired => redisExpired;

        public string JWTPassword => jWTPassword;

        public string JWT => jWT;

        public bool IsExtraSub => isExtraSub;

        public int MaxMonthsGetAssociationByDevice => maxMonthsGetAssociationByDevice;

        public long EverTaggingExpireSecond => everTaggingExpireSecond;

        public int EverTaggingDBIndex => everTaggingDBIndex;

        public long ApiEverTaggingCacheMaxAge => apiEverTaggingCacheMaxAge;

        public bool IsDisableJsChallenge => isDisableJsChallenge;

        public bool IsVaultEnable => isVaultEnable;

        public T GetValue<T>(string key)
        {
            try
            {
                var value = configuration[key];

                var typeConverter = TypeDescriptor.GetConverter(typeof(T));

                return (T)typeConverter.ConvertFromString(value);
            }
            catch (Exception ex)
            {
                throw new InvalidCastException($"Key: {key}", ex);
            }
        }

        public T GetSection<T>(string key)
        {
            var schedulerSection = configuration.GetSection(key);

            return schedulerSection.Get<T>();
        }

        public IConfiguration Configuration => configuration;

        public string JWTIssue => jWTIssue;

        public string JWTAudience => jWTAudience;
    }
}

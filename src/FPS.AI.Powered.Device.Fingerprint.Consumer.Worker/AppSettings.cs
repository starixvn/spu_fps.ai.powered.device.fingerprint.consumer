using System.ComponentModel;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker
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

        private readonly int maxMonthsGetAssociationByDevice;

        private readonly long everTaggingExpireSecond;

        private readonly int everTaggingDBIndex;

        private readonly long apiEverTaggingCacheMaxAge;

        private readonly bool isDisableJsChallenge;

        private readonly bool isVaultEnable;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            apiUrl = GetValue<string>("AppConfig:ApiUrl") ?? string.Empty;

            cdnApiUrl = GetValue<string>("AppConfig:CDNApiUrl") ?? string.Empty;

            portalApiUrl = GetValue<string>("AppConfig:PortalApiUrl") ?? string.Empty;

            cryptoPassword = GetValue<string>("AppConfig:Crypto:Password") ?? string.Empty;

            cryptoIV = GetValue<string>("AppConfig:Crypto:IV") ?? string.Empty;

            cryptoSalt = GetValue<string>("AppConfig:Crypto:Salt") ?? string.Empty;

            spName = GetValue<string>("StoredProcedurePath") ?? "App_Data";

            sentryDsn = GetValue<string>("Sentry:Dsn") ?? string.Empty;

            environment = GetValue<string>("AppConfig:Environment") ?? string.Empty;

            redisDatabase = Convert.ToInt32(string.IsNullOrEmpty(GetValue<string>("Redis:Database")) ? "0" : GetValue<string>("Redis:Database"));

            redisExpired = Convert.ToInt32(string.IsNullOrEmpty(GetValue<string>("Redis:Expired")) ? "3600" : GetValue<string>("Redis:Expired"));

            jWTPassword = GetValue<string>("AppConfig:JWT:Password") ?? "!QAZxdr567890";

            jWTIssue = GetValue<string>("AppConfig:JWT:Issue") ?? string.Empty;

            jWTAudience = GetValue<string>("AppConfig:JWT:Audience") ?? string.Empty;

            jWT = GetValue<string>("AppConfig:JWT") ?? string.Empty;

            maxMonthsGetAssociationByDevice = Convert.ToInt32(string.IsNullOrEmpty(GetValue<string>("API:MaxMonthsGetAssociationByDevice")) ? "0" : GetValue<string>("API:MaxMonthsGetAssociationByDevice"));

            everTaggingExpireSecond = Convert.ToInt64(string.IsNullOrEmpty(GetValue<string>("Redis:EverTaggingExpireSecond")) ? "3600" : GetValue<string>("Redis:EverTaggingExpireSecond"));

            everTaggingDBIndex = Convert.ToInt32(string.IsNullOrEmpty(GetValue<string>("Redis:EverTaggingDbIndex")) ? "0" : GetValue<string>("Redis:EverTaggingDbIndex"));

            apiEverTaggingCacheMaxAge = Convert.ToInt64(string.IsNullOrEmpty(GetValue<string>("ApiEverTaggingCacheMaxAge")) ? "86400" : GetValue<string>("ApiEverTaggingCacheMaxAge"));

            isDisableJsChallenge = !string.IsNullOrEmpty(GetValue<string>("Consumer:IsDisableJsChallenge")) && Convert.ToBoolean(GetValue<string>("Consumer:IsDisableJsChallenge"));

            isVaultEnable = !string.IsNullOrEmpty(GetValue<string>("VaultOptions:Enable")) && Convert.ToBoolean(GetValue<string>("VaultOptions:Enable"));
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

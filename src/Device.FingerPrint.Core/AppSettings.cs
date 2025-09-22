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

        int MaxMonthsGetAssociationByDevice { get; }

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

        private readonly int maxMonthsGetAssociationByDevice;

        private readonly bool isDisableJsChallenge;

        private readonly bool isVaultEnable;

        public AppSettings(IConfiguration configuration)
        {
            this.configuration = configuration;

            // Initialize all settings in constructor to avoid readonly field assignment issues
            var (apiUrlVal, cdnApiUrlVal, portalApiUrlVal, spNameVal, environmentVal, sentryDsnVal) = GetApiSettings();
            var (cryptoPasswordVal, cryptoIVVal, cryptoSaltVal) = GetCryptoSettings();
            var (maxMonthsGetAssociationByDeviceVal, isDisableJsChallengeVal) = GetConsumerSettings();
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

        private (int maxMonthsGetAssociationByDevice, bool isDisableJsChallenge) GetConsumerSettings()
        {
            var maxMonthsValue = GetValue<string>("API:MaxMonthsGetAssociationByDevice");
            var isDisableJsChallengeValue = GetValue<string>("Consumer:IsDisableJsChallenge");

            return (
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

        public int MaxMonthsGetAssociationByDevice => maxMonthsGetAssociationByDevice;

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
    }
}

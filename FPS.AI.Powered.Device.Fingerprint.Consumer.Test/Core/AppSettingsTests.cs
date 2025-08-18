using Device.FingerPrint.Core;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Core
{
    public class AppSettingsTests
    {
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AppSettings _appSettings;

        public AppSettingsTests()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            
            // Setup default configuration values
            _mockConfiguration.Setup(x => x["ApiUrl"]).Returns("https://api.example.com");
            _mockConfiguration.Setup(x => x["PortalApiUrl"]).Returns("https://portal.example.com");
            _mockConfiguration.Setup(x => x["CryptoPassword"]).Returns("test-crypto-password");
            _mockConfiguration.Setup(x => x["CryptoIV"]).Returns("c3eef06037e837f4ee390bda69c05023");
            _mockConfiguration.Setup(x => x["CryptoSalt"]).Returns("234292a9fce031f2");
            _mockConfiguration.Setup(x => x["SPName"]).Returns("test-sp-name");
            _mockConfiguration.Setup(x => x["SentryDsn"]).Returns("https://test-sentry-dsn");
            _mockConfiguration.Setup(x => x["Environment"]).Returns("TEST");
            _mockConfiguration.Setup(x => x["RedisDatabase"]).Returns("1");
            _mockConfiguration.Setup(x => x["RedisExpired"]).Returns("3600");
            _mockConfiguration.Setup(x => x["JWTPassword"]).Returns("test-jwt-password");
            _mockConfiguration.Setup(x => x["JWT"]).Returns("test-jwt");
            _mockConfiguration.Setup(x => x["JWTIssue"]).Returns("test-issuer");
            _mockConfiguration.Setup(x => x["JWTAudience"]).Returns("test-audience");
            _mockConfiguration.Setup(x => x["IsEnableLogstash"]).Returns("true");
            _mockConfiguration.Setup(x => x["LogstashTcpHostname"]).Returns("localhost");
            _mockConfiguration.Setup(x => x["LogstashTcpPort"]).Returns("5000");
            _mockConfiguration.Setup(x => x["IsExtraSub"]).Returns("false");
            _mockConfiguration.Setup(x => x["MaxMonthsGetAssociationByDevice"]).Returns("12");
            _mockConfiguration.Setup(x => x["EverTaggingExpireSecond"]).Returns("86400");
            _mockConfiguration.Setup(x => x["EverTaggingDBIndex"]).Returns("2");
            _mockConfiguration.Setup(x => x["ApiEverTaggingCacheMaxAge"]).Returns("3600");
            _mockConfiguration.Setup(x => x["IsDisableJsChallenge"]).Returns("false");
            _mockConfiguration.Setup(x => x["IsVaultEnable"]).Returns("false");

            _appSettings = new AppSettings(_mockConfiguration.Object);
        }

        [Fact]
        public void Constructor_WithValidConfiguration_ShouldInitializeProperties()
        {
            // Assert
            _appSettings.Should().NotBeNull();
            _appSettings.Configuration.Should().Be(_mockConfiguration.Object);
        }

        [Fact]
        public void ApiUrl_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.ApiUrl;

            // Assert
            result.Should().Be("https://api.example.com");
        }

        [Fact]
        public void PortalApiUrl_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.PortalApiUrl;

            // Assert
            result.Should().Be("https://portal.example.com");
        }

        [Fact]
        public void CryptoPassword_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.CryptoPassword;

            // Assert
            result.Should().Be("test-crypto-password");
        }

        [Fact]
        public void CryptoIV_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.CryptoIV;

            // Assert
            result.Should().Be("c3eef06037e837f4ee390bda69c05023");
        }

        [Fact]
        public void CryptoSalt_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.CryptoSalt;

            // Assert
            result.Should().Be("234292a9fce031f2");
        }

        [Fact]
        public void SPName_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.SPName;

            // Assert
            result.Should().Be("test-sp-name");
        }

        [Fact]
        public void SentryDsn_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.SentryDsn;

            // Assert
            result.Should().Be("https://test-sentry-dsn");
        }

        [Fact]
        public void Environment_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.Environment;

            // Assert
            result.Should().Be("TEST");
        }

        [Fact]
        public void RedisDatabase_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.RedisDatabase;

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public void RedisExpired_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.RedisExpired;

            // Assert
            result.Should().Be(3600);
        }

        [Fact]
        public void JWTPassword_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.JWTPassword;

            // Assert
            result.Should().Be("test-jwt-password");
        }

        [Fact]
        public void JWT_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.JWT;

            // Assert
            result.Should().Be("test-jwt");
        }

        [Fact]
        public void JWTIssue_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.JWTIssue;

            // Assert
            result.Should().Be("test-issuer");
        }

        [Fact]
        public void JWTAudience_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.JWTAudience;

            // Assert
            result.Should().Be("test-audience");
        }

        [Fact]
        public void IsEnableLogstash_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.IsEnableLogstash;

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void LogstashTcpHostname_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.LogstashTcpHostname;

            // Assert
            result.Should().Be("localhost");
        }

        [Fact]
        public void LogstashTcpPort_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.LogstashTcpPort;

            // Assert
            result.Should().Be(5000);
        }

        [Fact]
        public void IsExtraSub_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.IsExtraSub;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void MaxMonthsGetAssociationByDevice_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.MaxMonthsGetAssociationByDevice;

            // Assert
            result.Should().Be(12);
        }

        [Fact]
        public void EverTaggingExpireSecond_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.EverTaggingExpireSecond;

            // Assert
            result.Should().Be(86400);
        }

        [Fact]
        public void EverTaggingDBIndex_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.EverTaggingDBIndex;

            // Assert
            result.Should().Be(2);
        }

        [Fact]
        public void ApiEverTaggingCacheMaxAge_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.ApiEverTaggingCacheMaxAge;

            // Assert
            result.Should().Be(3600);
        }

        [Fact]
        public void IsDisableJsChallenge_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.IsDisableJsChallenge;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsVaultEnable_ShouldReturnConfiguredValue()
        {
            // Act
            var result = _appSettings.IsVaultEnable;

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetValue_WithStringKey_ShouldReturnStringValue()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["TestKey"]).Returns("test-value");

            // Act
            var result = _appSettings.GetValue<string>("TestKey");

            // Assert
            result.Should().Be("test-value");
        }

        [Fact]
        public void GetValue_WithIntKey_ShouldReturnIntValue()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["TestIntKey"]).Returns("123");

            // Act
            var result = _appSettings.GetValue<int>("TestIntKey");

            // Assert
            result.Should().Be(123);
        }

        [Fact]
        public void GetValue_WithBoolKey_ShouldReturnBoolValue()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["TestBoolKey"]).Returns("true");

            // Act
            var result = _appSettings.GetValue<bool>("TestBoolKey");

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void GetValue_WithMissingKey_ShouldReturnDefaultValue()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["MissingKey"]).Returns((string)null);

            // Act
            var result = _appSettings.GetValue<string>("MissingKey");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetValue_WithInvalidIntValue_ShouldReturnDefaultValue()
        {
            // Arrange
            _mockConfiguration.Setup(x => x["InvalidIntKey"]).Returns("not-a-number");

            // Act
            var result = _appSettings.GetValue<int>("InvalidIntKey");

            // Assert
            result.Should().Be(0); // Default value for int
        }

        [Fact]
        public void UnhandledFingerprintAttributeValues_ShouldReturnEmptyList_WhenNotConfigured()
        {
            // Act
            var result = _appSettings.UnhandledFingerprintAttributeValues;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void LogstashIgnoreSubscribers_ShouldReturnEmptyList_WhenNotConfigured()
        {
            // Act
            var result = _appSettings.LogstashIgnoreSubscribers;

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void DeviceIdEncryptKey_ShouldReturnConstantValue()
        {
            // Act
            var result = AppSettings.DeviceIdEncryptKey;

            // Assert
            result.Should().Be("&％#@?,:*!~^.");
        }
    }
}

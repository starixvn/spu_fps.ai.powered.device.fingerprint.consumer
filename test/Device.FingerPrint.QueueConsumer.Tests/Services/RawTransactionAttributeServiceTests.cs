using Xunit;
using Moq;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using Device.FingerPrint.Features.LogInfo.Services;
using Device.FingerPrint.Redis;
using StackExchange.Redis;
using System;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class RawTransactionAttributeServiceTests
    {
        private readonly Mock<ICrypto> _mockCrypto;
        private readonly Mock<IRedisMultiplexDbService> _mockRedisService;
        private readonly Mock<ILogInfoServices> _mockLogInfoServices;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly RawTransactionAttributeService _service;

        public RawTransactionAttributeServiceTests()
        {
            _mockCrypto = new Mock<ICrypto>();
            _mockRedisService = new Mock<IRedisMultiplexDbService>();
            _mockLogInfoServices = new Mock<ILogInfoServices>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockDatabase = new Mock<IDatabase>();

            // Setup default values
            _mockAppSettings.Setup(x => x.Environment).Returns("Test");
            _mockAppSettings.Setup(x => x.EverTaggingDBIndex).Returns(0);
            _mockAppSettings.Setup(x => x.RedisDatabase).Returns(0);
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:LogInfo")).Returns(false);
            _mockAppSettings.Setup(x => x.GetSection<string>("Consumer:ScriptVersion")).Returns("1.0.0");
            
            _mockRedisService.Setup(x => x.GetDatabase(It.IsAny<int>())).Returns(_mockDatabase.Object);
            _mockCrypto.Setup(x => x.GetMD5Hash(It.IsAny<string>())).Returns("mock-hash");

            _service = new RawTransactionAttributeService(
                _mockCrypto.Object,
                _mockRedisService.Object,
                _mockLogInfoServices.Object,
                _mockAppSettings.Object);
        }

        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            Assert.NotNull(_service);
        }

        [Fact]
        public void GetTransactionAttribute_WithNullModel_Should_Return_Null()
        {
            // Act
            var result = _service.GetTransactionAttribute(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetTransactionAttribute_WithValidModel_Should_Set_UserAgentInfo()
        {
            // Arrange
            var model = new RawTransactionModel
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36",
                SessionKey = null
            };

            // Act
            var result = _service.GetTransactionAttribute(model);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAgentInfo);
            Assert.Equal(model.UserAgent, result.RawUserAgent);
            Assert.Equal("mock-hash", result.FPUserAgentCode);
        }

        [Fact]
        public void GetTransactionAttribute_WithEmptySessionKey_Should_Handle_Gracefully()
        {
            // Arrange
            var model = new RawTransactionModel
            {
                UserAgent = "Mozilla/5.0 test",
                SessionKey = ""
            };

            // Act
            var result = _service.GetTransactionAttribute(model);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAgentInfo);
        }

        [Fact]
        public void GetTransactionAttribute_WithInvalidSessionKey_Should_Handle_Gracefully()
        {
            // Arrange
            var model = new RawTransactionModel
            {
                UserAgent = "Mozilla/5.0 test",
                SessionKey = "invalid-session-key-without-colon"
            };

            // Act
            var result = _service.GetTransactionAttribute(model);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAgentInfo);
        }

        [Fact]
        public void GetTransactionAttribute_WithException_Should_Handle_Gracefully()
        {
            // Arrange
            var model = new RawTransactionModel
            {
                UserAgent = "Mozilla/5.0 test",
                SessionKey = "0:valid-session-key"
            };

            _mockRedisService.Setup(x => x.HashGetWithDbIndex(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>()))
                           .Throws(new Exception("Redis error"));

            // Act
            var result = _service.GetTransactionAttribute(model);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetTransactionAttribute_WithOldScriptVersion_Should_Handle_Version_Check()
        {
            // Arrange
            var model = new RawTransactionModel
            {
                UserAgent = "Mozilla/5.0 test",
                ScriptVersion = "0.9.0", // Older version
                SessionKey = null
            };

            _mockAppSettings.Setup(x => x.GetSection<string>("Consumer:ScriptVersion")).Returns("1.0.0");

            // Act
            var result = _service.GetTransactionAttribute(model);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAgentInfo);
        }

        [Fact]
        public void GetTransactionAttribute_WithInvalidScriptVersion_Should_Truncate_Long_Version()
        {
            // Arrange
            var model = new RawTransactionModel
            {
                UserAgent = "Mozilla/5.0 test",
                ScriptVersion = "very-long-invalid-script-version-string-that-exceeds-limit", // Invalid long version
                SessionKey = null
            };

            // Act
            var result = _service.GetTransactionAttribute(model);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.ScriptVersion.Length <= 10);
        }

        [Fact]
        public void GetTransactionAttribute_WithEmptyUserAgent_Should_Create_Empty_UserAgentInfo()
        {
            // Arrange
            var model = new RawTransactionModel
            {
                UserAgent = "",
                SessionKey = null
            };

            // Act
            var result = _service.GetTransactionAttribute(model);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.UserAgentInfo);
            Assert.Equal("", result.RawUserAgent);
        }
    }
}

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
using System.Reflection;
using System.Collections.Generic;
using Device.FingerPrint.Model.DTO;

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

        #region Private Static Utility Methods Tests

        [Theory]
        [InlineData("1.0.0", "2.0.0", 0)] // Same major, older minor - should return 0
        [InlineData("1.5.0", "1.3.0", 1)] // Same major, newer minor - should return 1
        [InlineData("2.0.0", "1.9.9", 0)] // Different major - should return 0
        [InlineData("1.0.0", "1.0.0", 1)] // Same version - should return 1
        [InlineData("invalid", "1.0.0", -1)] // Invalid versionA - should return -1
        [InlineData("1.0.0", "invalid", -1)] // Invalid versionB - should return -1
        [InlineData("invalid", "invalid", -1)] // Both invalid - should return -1
        [InlineData(null, "1.0.0", -1)] // Null versionA - should return -1
        [InlineData("1.0.0", null, -1)] // Null versionB - should return -1
        public void CompareVersionsWithTryParse_Should_Return_Expected_Result(string versionA, string versionB, int expected)
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("CompareVersionsWithTryParse", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Act
            var result = method.Invoke(null, new object[] { versionA, versionB });
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, (int)result);
        }

        [Theory]
        [InlineData("1234567890ABCDEF", true)]
        [InlineData("abcdef123456", true)]
        [InlineData("ABCDEF123456", true)]
        [InlineData("123456", true)]
        [InlineData("abc", true)]
        [InlineData("ABC", true)]
        [InlineData("123G456", false)] // Contains 'G' which is not hex
        [InlineData("123 456", false)] // Contains space
        [InlineData("123-456", false)] // Contains dash
        [InlineData("", false)] // Empty string
        [InlineData(null, false)] // Null string
        [InlineData("xyz", false)] // Non-hex characters
        public void IsHexString_Should_Return_Expected_Result(string input, bool expected)
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("IsHexString", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Act
            var result = method.Invoke(null, new object[] { input });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, (bool)result);
        }

        [Theory]
        [InlineData("12345678901234567890123456789012", true)] // Valid 32-char GUID format
        [InlineData("550e8400e29b41d4a716446655440000", true)] // Valid GUID without dashes
        [InlineData("12345678-1234-1234-1234-123456789012", false)] // 36 chars with dashes (wrong length)
        [InlineData("1234567890123456789012345678901", false)] // 31 chars (too short)
        [InlineData("123456789012345678901234567890123", false)] // 33 chars (too long)
        [InlineData("", false)] // Empty string
        [InlineData(null, false)] // Null string
        [InlineData("1234567890123456789012345678901G", false)] // 32 chars but invalid GUID
        public void CheckGuid_Should_Return_Expected_Result(string input, bool expected)
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("CheckGuid", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Act
            var result = method.Invoke(null, new object[] { input });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, (bool)result);
        }

        [Theory]
        [InlineData("Mozilla/5.0 Test Browser", "Mozilla/5.0 Test Browser")]
        [InlineData("Mozilla/5.0 UUID/123-456-789 Test", "Mozilla/5.0 Test")]
        [InlineData("Mozilla/5.0 HTTP_BB_FORWARDED/somevalue Test", "Mozilla/5.0 Test")]
        [InlineData("Mozilla/5.0 FalconTag/tagvalue Test", "Mozilla/5.0 Test")]
        [InlineData("Mozilla/5.0 SessionId/sessionvalue Test", "Mozilla/5.0 Test")]
        [InlineData("Mozilla/5.0 [some brackets] Test", "Mozilla/5.0 Test")]
        [InlineData("Mozilla/5.0   Multiple   Spaces", "Mozilla/5.0 Multiple Spaces")]
        [InlineData("", "")]
        public void CleanUserAgentFuzzy_Should_Clean_UserAgent_Correctly(string input, string expected)
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("CleanUserAgentFuzzy", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Act
            var result = method.Invoke(null, new object[] { input });

            // Assert
            Assert.Equal(expected, result as string);
        }

        [Fact]
        public void CleanUserAgentFuzzy_Should_Handle_Null_Input()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("CleanUserAgentFuzzy", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Act
            var result = method.Invoke(null, new object?[] { null });

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("Chrome", false)]
        [InlineData("Firefox", false)]
        [InlineData("Safari", false)]
        [InlineData("uc browser", true)]
        [InlineData("UC Browser", true)]
        [InlineData("UC BROWSER", true)]
        [InlineData("miui browser", true)]
        [InlineData("MIUI Browser", true)]
        [InlineData("MIUI BROWSER", true)]
        [InlineData("Chrome UC Browser", true)]
        [InlineData("Firefox MIUI Browser", true)]
        [InlineData("", false)]
        [InlineData(null, false)]
        public void IgnoreDetectPrivateMode_Should_Return_Expected_Result(string browserName, bool expected)
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("IgnoreDetectPrivateMode", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Act
            var result = method.Invoke(null, new object[] { browserName });

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, (bool)result);
        }

        [Fact]
        public void GetSuperCookies_Should_Return_Correct_Format_With_All_True()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("GetSuperCookies", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            
            var fingerprintModel = new FPFingerprintAttributeModel
            {
                LocalStorage = "true",
                SessionStorage = "true",
                OpenDatabase = "true",
                IndexedDB = "true"
            };

            // Act
            var result = method.Invoke(null, new object[] { fingerprintModel });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("True|True|True|True", result as string);
        }

        [Fact]
        public void GetSuperCookies_Should_Return_Correct_Format_With_All_False()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("GetSuperCookies", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            
            var fingerprintModel = new FPFingerprintAttributeModel
            {
                LocalStorage = "false",
                SessionStorage = "false",
                OpenDatabase = "false",
                IndexedDB = "false"
            };

            // Act
            var result = method.Invoke(null, new object[] { fingerprintModel });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("False|False|False|False", result as string);
        }

        [Fact]
        public void GetSuperCookies_Should_Handle_Null_FingerprintModel()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("GetSuperCookies", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);

            // Act
            var result = method.Invoke(null, new object?[] { null });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("False|False|False|False", result as string);
        }

        [Fact]
        public void GetSuperCookies_Should_Handle_Mixed_Values()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("GetSuperCookies", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            
            var fingerprintModel = new FPFingerprintAttributeModel
            {
                LocalStorage = "true",
                SessionStorage = "false",
                OpenDatabase = "true",
                IndexedDB = null
            };

            // Act
            var result = method.Invoke(null, new object[] { fingerprintModel });

            // Assert
            Assert.NotNull(result);
            Assert.Equal("True|False|True|False", result as string);
        }

        [Fact]
        public void HasData_Should_Return_True_When_Model_Has_Data()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("HasData", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            var genericMethod = method.MakeGenericMethod(typeof(TestModel));
            
            var model = new TestModel { Name = "Test", Value = 123 };

            // Act
            var result = genericMethod.Invoke(null, new object[] { model });

            // Assert
            Assert.NotNull(result);
            Assert.True((bool)result);
        }

        [Fact]
        public void HasData_Should_Return_False_When_Model_Has_No_Data()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("HasData", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            var genericMethod = method.MakeGenericMethod(typeof(TestModel));
            
            var model = new TestModel { Name = null, Value = 0 };

            // Act
            var result = genericMethod.Invoke(null, new object[] { model });

            // Assert
            Assert.NotNull(result);
            Assert.False((bool)result);
        }

        [Fact]
        public void HasData_Should_Return_False_When_Model_Has_Empty_String()
        {
            // Arrange
            var method = typeof(RawTransactionAttributeService).GetMethod("HasData", 
                BindingFlags.NonPublic | BindingFlags.Static);
            Assert.NotNull(method);
            var genericMethod = method.MakeGenericMethod(typeof(TestModel));
            
            var model = new TestModel { Name = "", Value = 0 };

            // Act
            var result = genericMethod.Invoke(null, new object[] { model });

            // Assert
            Assert.NotNull(result);
            Assert.False((bool)result);
        }

        // Test model class for HasData tests
        public class TestModel
        {
            public string? Name { get; set; }
            public int Value { get; set; }
        }

        #endregion
    }
}

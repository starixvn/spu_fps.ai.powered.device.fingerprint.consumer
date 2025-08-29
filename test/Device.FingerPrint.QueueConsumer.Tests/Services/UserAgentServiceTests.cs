using Xunit;
using Moq;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using System;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class UserAgentServiceTests
    {
        private readonly Mock<ICrypto> _mockCrypto;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly UserAgentService _service;

        public UserAgentServiceTests()
        {
            _mockCrypto = new Mock<ICrypto>();
            _mockAppSettings = new Mock<IAppSettings>();

            // Setup default values
            _mockAppSettings.Setup(x => x.Environment).Returns("Test");
            _mockCrypto.Setup(x => x.GetMD5Hash(It.IsAny<string>())).Returns("mock-hash-value");

            _service = new UserAgentService(_mockCrypto.Object, _mockAppSettings.Object);
        }

        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            Assert.NotNull(_service);
        }

        [Fact]
        public void GetUserAgentInfo_WithNullUserAgent_Should_Return_Empty_Model()
        {
            // Act
            var result = _service.GetUserAgentInfo(null);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.UserAgent);
        }

        [Fact]
        public void GetUserAgentInfo_WithEmptyUserAgent_Should_Return_Empty_Model()
        {
            // Act
            var result = _service.GetUserAgentInfo("");

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.UserAgent);
        }

        [Fact]
        public void GetUserAgentInfo_WithValidChromeUserAgent_Should_Parse_Correctly()
        {
            // Arrange
            var chromeUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(chromeUserAgent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(chromeUserAgent, result.UserAgent);
            Assert.Equal("mock-hash-value", result.FPUserAgentCode);
            Assert.NotNull(result.ClientName);
            Assert.NotNull(result.OSName);
            Assert.True(result.DeviceType >= 0);
        }

        [Fact]
        public void GetUserAgentInfo_WithValidFirefoxUserAgent_Should_Parse_Correctly()
        {
            // Arrange
            var firefoxUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0";

            // Act
            var result = _service.GetUserAgentInfo(firefoxUserAgent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(firefoxUserAgent, result.UserAgent);
            Assert.Equal("mock-hash-value", result.FPUserAgentCode);
            Assert.NotNull(result.ClientName);
            Assert.NotNull(result.OSName);
        }

        [Fact]
        public void GetUserAgentInfo_WithMobileUserAgent_Should_Detect_Mobile()
        {
            // Arrange
            var mobileUserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Mobile/15E148 Safari/604.1";

            // Act
            var result = _service.GetUserAgentInfo(mobileUserAgent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mobileUserAgent, result.UserAgent);
            Assert.Equal("mock-hash-value", result.FPUserAgentCode);
            Assert.Equal(3, result.DeviceType); // Mobile = 3
        }

        [Fact]
        public void GetUserAgentInfo_WithTabletUserAgent_Should_Detect_Tablet()
        {
            // Arrange
            var tabletUserAgent = "Mozilla/5.0 (iPad; CPU OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Mobile/15E148 Safari/604.1";

            // Act
            var result = _service.GetUserAgentInfo(tabletUserAgent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(tabletUserAgent, result.UserAgent);
            Assert.Equal("mock-hash-value", result.FPUserAgentCode);
            Assert.Equal(2, result.DeviceType); // Tablet = 2
        }

        [Fact]
        public void GetUserAgentInfo_WithDesktopUserAgent_Should_Detect_Desktop()
        {
            // Arrange
            var desktopUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(desktopUserAgent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(desktopUserAgent, result.UserAgent);
            Assert.Equal("mock-hash-value", result.FPUserAgentCode);
            Assert.Equal(1, result.DeviceType); // Desktop = 1
        }

        [Fact]
        public void GetUserAgentInfo_WithInvalidUserAgent_Should_Return_Basic_Model()
        {
            // Arrange
            var invalidUserAgent = "InvalidUserAgentString";

            // Act
            var result = _service.GetUserAgentInfo(invalidUserAgent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(invalidUserAgent, result.UserAgent);
            Assert.Equal("mock-hash-value", result.FPUserAgentCode);
            Assert.Equal(0, result.DeviceType); // Other = 0
        }


        [Fact]
        public void GetUserAgentInfo_Should_Call_Crypto_GetMD5Hash()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 test";

            // Act
            _service.GetUserAgentInfo(userAgent);

            // Assert
            _mockCrypto.Verify(x => x.GetMD5Hash(userAgent), Times.Once);
        }

        [Fact]
        public void GetUserAgentInfo_WithAndroidUserAgent_Should_Parse_Android_Info()
        {
            // Arrange
            var androidUserAgent = "Mozilla/5.0 (Linux; Android 11; SM-G991B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.120 Mobile Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(androidUserAgent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(androidUserAgent, result.UserAgent);
            Assert.Equal("mock-hash-value", result.FPUserAgentCode);
            Assert.Equal(3, result.DeviceType); // Mobile = 3
            Assert.NotNull(result.OSName);
            Assert.NotNull(result.ClientName);
        }
    }
}

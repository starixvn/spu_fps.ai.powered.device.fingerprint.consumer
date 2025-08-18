using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Services
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

            _mockCrypto.Setup(x => x.GetMD5Hash(It.IsAny<string>())).Returns("test-md5-hash");
            _mockAppSettings.Setup(x => x.Environment).Returns("TEST");

            _service = new UserAgentService(_mockCrypto.Object, _mockAppSettings.Object);
        }

        [Fact]
        public void GetUserAgentInfo_WithNullUserAgent_ShouldReturnEmptyModel()
        {
            // Arrange
            string userAgent = null;

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().BeNull();
            result.ClientType.Should().BeNull();
            result.ClientName.Should().BeNull();
        }

        [Fact]
        public void GetUserAgentInfo_WithEmptyUserAgent_ShouldReturnEmptyModel()
        {
            // Arrange
            var userAgent = "";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().BeNull();
            result.ClientType.Should().BeNull();
            result.ClientName.Should().BeNull();
        }

        [Fact]
        public void GetUserAgentInfo_WithValidDesktopUserAgent_ShouldReturnDesktopInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.DeviceType.Should().Be(1); // Desktop
            result.FPUserAgentCode.Should().Be("test-md5-hash");
            _mockCrypto.Verify(x => x.GetMD5Hash(userAgent), Times.Once);
        }

        [Fact]
        public void GetUserAgentInfo_WithValidMobileUserAgent_ShouldReturnMobileInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Mobile/15E148 Safari/604.1";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.DeviceType.Should().Be(3); // Mobile
            result.FPUserAgentCode.Should().Be("test-md5-hash");
        }

        [Fact]
        public void GetUserAgentInfo_WithValidTabletUserAgent_ShouldReturnTabletInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (iPad; CPU OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Mobile/15E148 Safari/604.1";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.DeviceType.Should().Be(2); // Tablet
            result.FPUserAgentCode.Should().Be("test-md5-hash");
        }

        [Fact]
        public void GetUserAgentInfo_WithChromeBrowser_ShouldReturnChromeInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.ClientName.Should().Be("Chrome");
            result.ClientType.Should().Be("browser");
        }

        [Fact]
        public void GetUserAgentInfo_WithFirefoxBrowser_ShouldReturnFirefoxInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:89.0) Gecko/20100101 Firefox/89.0";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.ClientName.Should().Be("Firefox");
            result.ClientType.Should().Be("browser");
        }

        [Fact]
        public void GetUserAgentInfo_WithWindowsOS_ShouldReturnWindowsInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.OSName.Should().Be("Windows");
            result.OSShortName.Should().Be("WIN");
            result.OSPlatform.Should().Be("x64");
        }

        [Fact]
        public void GetUserAgentInfo_WithMacOS_ShouldReturnMacInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.OSName.Should().Be("macOS");
            result.OSShortName.Should().Be("MAC");
        }

        [Fact]
        public void GetUserAgentInfo_WithiOS_ShouldReturniOSInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.1.1 Mobile/15E148 Safari/604.1";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.OSName.Should().Be("iOS");
            result.OSShortName.Should().Be("IOS");
        }

        [Fact]
        public void GetUserAgentInfo_WithAndroid_ShouldReturnAndroidInfo()
        {
            // Arrange
            var userAgent = "Mozilla/5.0 (Linux; Android 11; SM-G991B) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.120 Mobile Safari/537.36";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.OSName.Should().Be("Android");
            result.OSShortName.Should().Be("AND");
        }

        [Fact]
        public void GetUserAgentInfo_WithUnknownUserAgent_ShouldReturnOtherDeviceType()
        {
            // Arrange
            var userAgent = "Unknown/1.0";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.DeviceType.Should().Be(0); // Other
            result.FPUserAgentCode.Should().Be("test-md5-hash");
        }

        [Fact]
        public void GetUserAgentInfo_WithException_ShouldReturnEmptyModel()
        {
            // Arrange
            _mockCrypto.Setup(x => x.GetMD5Hash(It.IsAny<string>())).Throws(new Exception("Test exception"));
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";

            // Act
            var result = _service.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().BeNull();
            result.ClientType.Should().BeNull();
            result.ClientName.Should().BeNull();
        }
    }
}

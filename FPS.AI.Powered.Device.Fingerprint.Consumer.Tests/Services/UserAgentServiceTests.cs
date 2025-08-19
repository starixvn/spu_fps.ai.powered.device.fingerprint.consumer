using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using Device.FingerPrint.QueueConsumer.Services;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Tests.Services
{
    public class UserAgentServiceTests
    {
        private readonly UserAgentService _userAgentService;
        private readonly ICrypto _mockCrypto;
        private readonly IAppSettings _mockAppSettings;

        public UserAgentServiceTests()
        {
            _mockCrypto = Substitute.For<ICrypto>();
            _mockAppSettings = Substitute.For<IAppSettings>();
            _mockAppSettings.Environment.Returns("TEST");

            _userAgentService = new UserAgentService(_mockCrypto, _mockAppSettings);
        }

        [Fact]
        public void GetUserAgentInfo_WhenUserAgentIsNull_ShouldReturnEmptyUserAgentInfo()
        {
            // Arrange
            string? userAgent = null;

            // Act
            var result = _userAgentService.GetUserAgentInfo(userAgent!);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().BeNull();
            result.ClientType.Should().BeNull();
            result.ClientName.Should().BeNull();
            result.ClientVersion.Should().BeNull();
            result.OSName.Should().BeNull();
            result.OSShortName.Should().BeNull();
            result.OSVersion.Should().BeNull();
            result.OSPlatform.Should().BeNull();
            result.Device.Should().BeNull();
            result.Brand.Should().BeNull();
            result.Model.Should().BeNull();
            result.DeviceType.Should().Be(0);
            result.FPUserAgentCode.Should().BeNull();
        }

        [Fact]
        public void GetUserAgentInfo_WhenUserAgentIsEmpty_ShouldReturnEmptyUserAgentInfo()
        {
            // Arrange
            string userAgent = "";

            // Act
            var result = _userAgentService.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().BeNull();
            result.ClientType.Should().BeNull();
            result.ClientName.Should().BeNull();
            result.ClientVersion.Should().BeNull();
            result.OSName.Should().BeNull();
            result.OSShortName.Should().BeNull();
            result.OSVersion.Should().BeNull();
            result.OSPlatform.Should().BeNull();
            result.Device.Should().BeNull();
            result.Brand.Should().BeNull();
            result.Model.Should().BeNull();
            result.DeviceType.Should().Be(0);
            result.FPUserAgentCode.Should().BeNull();
        }

        [Fact]
        public void GetUserAgentInfo_WhenValidDesktopUserAgent_ShouldReturnDesktopInfo()
        {
            // Arrange
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
            _mockCrypto.GetMD5Hash(userAgent).Returns("test-hash-123");

            // Act
            var result = _userAgentService.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.ClientType.Should().NotBeNull();
            result.ClientName.Should().NotBeNull();
            result.ClientVersion.Should().NotBeNull();
            result.OSName.Should().NotBeNull();
            result.OSShortName.Should().NotBeNull();
            result.OSVersion.Should().NotBeNull();
            result.OSPlatform.Should().NotBeNull();
            result.DeviceType.Should().Be(1); // Desktop
            result.FPUserAgentCode.Should().Be("test-hash-123");
        }

        [Fact]
        public void GetUserAgentInfo_WhenValidMobileUserAgent_ShouldReturnMobileInfo()
        {
            // Arrange
            string userAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 14_6 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/14.0 Mobile/15E148 Safari/604.1";
            _mockCrypto.GetMD5Hash(userAgent).Returns("mobile-hash-456");

            // Act
            var result = _userAgentService.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.ClientType.Should().NotBeNull();
            result.ClientName.Should().NotBeNull();
            result.ClientVersion.Should().NotBeNull();
            result.OSName.Should().NotBeNull();
            result.OSShortName.Should().NotBeNull();
            result.OSVersion.Should().NotBeNull();
            result.OSPlatform.Should().NotBeNull();
            result.DeviceType.Should().Be(3); // Mobile
            result.FPUserAgentCode.Should().Be("mobile-hash-456");
        }

        [Fact]
        public void GetUserAgentInfo_WhenInvalidUserAgent_ShouldReturnEmptyUserAgentInfo()
        {
            // Arrange
            string userAgent = "Invalid User Agent String";

            // Act
            var result = _userAgentService.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.ClientType.Should().BeNull();
            result.ClientName.Should().BeNull();
            result.ClientVersion.Should().BeNull();
            result.OSName.Should().BeNull();
            result.OSShortName.Should().BeNull();
            result.OSVersion.Should().BeNull();
            result.OSPlatform.Should().BeNull();
            result.Device.Should().BeNull();
            result.Brand.Should().BeNullOrEmpty();
            result.Model.Should().BeNullOrEmpty();
            result.DeviceType.Should().Be(0); // Other
            result.FPUserAgentCode.Should().NotBeNull();
        }

        [Fact]
        public void GetUserAgentInfo_WhenExceptionOccurs_ShouldReturnEmptyUserAgentInfo()
        {
            // Arrange
            string userAgent = "Mozilla/5.0";
            _mockCrypto.GetMD5Hash(userAgent).Returns("test-hash");

            // Act
            var result = _userAgentService.GetUserAgentInfo(userAgent);

            // Assert
            result.Should().NotBeNull();
            result.UserAgent.Should().Be(userAgent);
            result.FPUserAgentCode.Should().Be("test-hash");
        }
    }
}

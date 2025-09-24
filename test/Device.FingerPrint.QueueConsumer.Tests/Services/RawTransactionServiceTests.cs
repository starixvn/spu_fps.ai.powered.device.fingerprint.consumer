using Device.FingerPrint.Core;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.QueueConsumer.Services;
using Moq;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class RawTransactionServiceTests
    {
        private readonly Mock<IBotDetectionService> _mockBotDetectionService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly RawTransactionService _service;

        public RawTransactionServiceTests()
        {
            _mockBotDetectionService = new Mock<IBotDetectionService>();
            _mockAppSettings = new Mock<IAppSettings>();

            _mockAppSettings.Setup(x => x.Environment).Returns("Test");

            _service = new RawTransactionService(
                _mockBotDetectionService.Object,
                _mockAppSettings.Object);
        }

        #region Happy Path Tests

        [Fact]
        public void TransformRawTransaction_WithValidMessage_Should_ReturnRawTransactionModel()
        {
            // Arrange
            var validGuid = Guid.NewGuid().ToString();
            var message = CreateValidMessageQueue(validGuid);
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(validGuid, result.DeviceCode);
            Assert.Equal("test_user", result.LoginName);
            Assert.Equal("test_subscriber", result.SubscriberName);
            Assert.Equal(123, result.SubscriberId);
            Assert.Equal("login", result.Action);
            Assert.Equal("success", result.ActionResult);
            Assert.Equal("192.168.1.1", result.IP);
            Assert.Equal(0, result.PluginID);
        }

        [Fact]
        public void TransformRawTransaction_WithValidMessage_Should_CallGenerateGroupHashCodes()
        {
            // Arrange
            var message = CreateValidMessageQueue();

            // Set up fingerprint data that will generate hash codes
            message.Fingerprint.Au = "test-audio";
            message.Fingerprint.Br = "test-browser";
            message.Fingerprint.Hw = "test-hardware";
            message.Fingerprint.Pf = "test-preferences";
            message.Fingerprint.Gp = "test-graphic";

            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.FingerPrint);

            // Verify that GenerateGroupHashCodes was called by checking the generated hash codes
            Assert.NotNull(message.Fingerprint.AudioCode);
            Assert.NotNull(message.Fingerprint.BrowserCode);
            Assert.NotNull(message.Fingerprint.HardwareCode);
            Assert.NotNull(message.Fingerprint.PreferencesCode);
            Assert.NotNull(message.Fingerprint.GraphicCode);

            // Verify hash codes are 32 characters long (as per the implementation)
            Assert.Equal(32, message.Fingerprint.AudioCode.Length);
            Assert.Equal(32, message.Fingerprint.BrowserCode.Length);
            Assert.Equal(32, message.Fingerprint.HardwareCode.Length);
            Assert.Equal(32, message.Fingerprint.PreferencesCode.Length);
            Assert.Equal(32, message.Fingerprint.GraphicCode.Length);
        }

        #endregion

        #region Exception Handling Tests

        [Fact]
        public void TransformRawTransaction_WithNullMessage_Should_ReturnNull()
        {
            // Act
            var result = _service.TransformRawTransaction(null);

            // Assert
            Assert.Null(result);
        }



        #endregion

        #region Device Validation Tests


        [Fact]
        public void TransformRawTransaction_WithValidGuidDeviceNo_Should_NotSetFlags()
        {
            // Arrange
            var validGuid = Guid.NewGuid().ToString();
            var message = CreateValidMessageQueue();
            message.DeviceInfo.DeviceNo = validGuid;
            message.DeviceInfo.InvalidDeviceId = false;
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(validGuid, result.DeviceCode);
            Assert.Null(result.InvalidDevice);
            Assert.Equal(0, result.TransStatus); // No flags set
        }

        #endregion

        #region User Agent Processing Tests

        [Fact]
        public void TransformRawTransaction_WithUserAgentContainingUnicode_Should_StripUnicode()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.UserAgent = "Mozilla/5.0 (Test) 🔥 Browser";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.DoesNotContain("🔥", result.UserAgent);
        }

        [Fact]
        public void TransformRawTransaction_WithNAUserAgent_Should_ConvertToEmpty()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.UserAgent = "N/A";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result.UserAgent);
        }

        [Fact]
        public void TransformRawTransaction_WithNullUserAgent_Should_HandleGracefully()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.UserAgent = null;
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("", result.UserAgent);
        }

        #endregion

        #region Bot Detection Tests







        #endregion

        #region IP Processing Tests

        [Fact]
        public void TransformRawTransaction_WithValidIP_Should_ProcessCorrectly()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.IP = "203.0.113.1";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("203.0.113.1", result.IP);
            Assert.True(result.IPId > 0);
            Assert.False(result.IsIPV6);
        }

        [Fact]
        public void TransformRawTransaction_WithIPContainingSpaces_Should_TrimSpaces()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.IP = "  192.168.1.1  ";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("192.168.1.1", result.IP);
        }

        [Fact]
        public void TransformRawTransaction_WithFakeIP_Should_SetFakeIP()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.FakeIP = "  198.51.100.1  ";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("198.51.100.1", result.FakeIP);
        }

        [Fact]
        public void TransformRawTransaction_WithNullIP_Should_HandleGracefully()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.IP = null;
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.IP);
            Assert.Equal(0, result.IPId);
        }

        #endregion

        #region URL Processing Tests

        [Fact]
        public void TransformRawTransaction_WithDeviceUrl_Should_UseDeviceUrl()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.Device.Url = "https://device.com/page";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("device.com", result.URL);
        }

        [Fact]
        public void TransformRawTransaction_WithoutDeviceUrl_Should_UseHRef()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.Device.Url = null;
            message.DeviceInfo.HRef = "https://fallback.com/page";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("fallback.com", result.URL);
        }

        [Fact]
        public void TransformRawTransaction_WithEmptyDeviceUrl_Should_UseHRef()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.Device.Url = "";
            message.DeviceInfo.HRef = "https://fallback.com/page";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Contains("fallback.com", result.URL);
        }

        #endregion

        #region Fingerprint Processing Tests

        [Fact]
        public void TransformRawTransaction_WithValidFingerprintCode_Should_SetFingerprintCode()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.DeviceFPCode = "VALID_FP_CODE";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("VALID_FP_CODE", result.FingerprintCode);
            Assert.NotNull(result.FingerprintMoreInfo);
        }

        [Fact]
        public void TransformRawTransaction_WithNAFingerprintCode_Should_SetNullFingerprint()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.DeviceFPCode = "N/A";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.FingerprintCode);
            Assert.Null(result.FingerprintMoreInfo);
        }

        #endregion

        #region WebRTC and Pattern Tests

        [Fact]
        public void TransformRawTransaction_WithWebRTCIP_Should_SetWebRTCIP()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.Rtc = "192.168.1.100";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.WebRTCIP);
        }

        [Fact]
        public void TransformRawTransaction_WithPatternCodes_Should_TrimAndSet()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.F4 = "  pattern1  ";
            message.DeviceInfo.F5 = "  pattern2  ";
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("pattern1", result.FPPatternCode01);
            Assert.Equal("pattern2", result.FPPatternCode02);
        }

        [Fact]
        public void TransformRawTransaction_WithNullPatternCodes_Should_HandleGracefully()
        {
            // Arrange
            var message = CreateValidMessageQueue();
            message.DeviceInfo.F4 = null;
            message.DeviceInfo.F5 = null;
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.FPPatternCode01);
            Assert.Null(result.FPPatternCode02);
        }

        #endregion

        #region Date Processing Tests

        [Fact]
        public void TransformRawTransaction_WithValidDate_Should_ConvertCorrectly()
        {
            // Arrange
            var testDate = new DateTime(2023, 12, 25, 14, 30, 45, 123);
            var message = CreateValidMessageQueue();
            message.TransDate = testDate;
            SetupBasicMocks();

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testDate, result.TransTime);
        }

        #endregion

        #region Helper Methods

        private MessageQueue CreateValidMessageQueue(string deviceNo = null)
        {
            return new MessageQueue
            {
                Account = "test_user",
                Subscriber = "test_subscriber",
                SubscriberId = 123,
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                FakeIP = null,
                IsNewFlow = false,
                DeviceInfo = new DeviceInfoModel
                {
                    DeviceNo = deviceNo ?? Guid.NewGuid().ToString(),
                    UserAgent = "Mozilla/5.0 Test Browser",
                    Version = "1.0",
                    DeviceFPCode = "TEST_FP",
                    HRef = "https://test.com",
                    Rtc = "192.168.1.100",
                    F4 = "pattern1",
                    F5 = "pattern2",
                    InvalidDeviceId = false,
                    Tagging = "test_tag"
                },
                Device = new DeviceModel
                {
                    Url = "https://test.com/page",
                    UserAgent = "Device User Agent",
                    Di = "device_identifier"
                },
                Fingerprint = new APFingerprintModel()
            };
        }

        private void SetupBasicMocks()
        {
            var botDModel = new BotDModel
            {
                IsVer3 = false,
                Detection = "none",
                Component = "test"
            };

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                                   .Returns(botDModel);
            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns("normal");
            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()))
                                   .Returns("none");
        }

        #endregion
    }

    // Additional enum for testing if not available
    public enum TransStatus
    {
        DeviceCodeIsEmpty = 0,
        InvalidDI = 1
    }
}
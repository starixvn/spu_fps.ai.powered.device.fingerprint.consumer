using Device.FingerPrint.Core;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Redis;
using Device.FingerPrint.TransactionAPI.Model;
using Moq;
using StackExchange.Redis;
using System.Collections;
using System.Numerics;
using Fanex.Logging;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class RawTransactionServiceTests
    {
        private readonly Mock<IDeviceService> _mockDeviceService;
        private readonly Mock<IBotDetectionService> _mockBotDetectionService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly RawTransactionService _rawTransactionService;

        public RawTransactionServiceTests()
        {
            _mockDeviceService = new Mock<IDeviceService>();
            _mockBotDetectionService = new Mock<IBotDetectionService>();
            _mockAppSettings = new Mock<IAppSettings>();

            // Setup logging engines for tests
            LogManager
                .SetDefaultLogCategory("Device.FingerPrint.Consumer.Tests")
                .Use(new List<ILogEngine>());

            // Setup default app settings
            _mockAppSettings.Setup(x => x.Environment).Returns("Test");

            _rawTransactionService = new RawTransactionService(
                _mockDeviceService.Object,
                _mockBotDetectionService.Object,
                _mockAppSettings.Object);
        }

        [Fact]
        public void TransformRawTransaction_WhenMessageIsNull_ReturnsNull()
        {
            // Act
            var result = _rawTransactionService.TransformRawTransaction(null!);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TransformRawTransaction_WhenMessageDeviceIsNull_CreatesNewDeviceModel()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testAccount",
                Subscriber = "testSubscriber",
                SubscriberId = 123,
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1"
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "valid-guid-device-id",
                Version = "2.0.1",
                Rtc = "119.82.130.208"
            };

            var botDModel = new BotDModel
            {
                Component = "testComponent",
                IsVer3 = true,
                Detection = "human"
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("flagged123");

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(message.Device);
            Assert.NotNull(message.Device.Di);
            Assert.Equal("testAccount", result.LoginName);
            Assert.Equal("testSubscriber", result.SubscriberName);
            Assert.Equal(123, result.SubscriberId);
        }

        [Fact]
        public void TransformRawTransaction_WithInvalidDeviceTagging_SetsTransStatusAndNullsTagging()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                Device = new DeviceModel { Di = "test-di" }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "invalid-device-id", // Not a valid GUID
                Version = "1.0.0"
            };

            var botDModel = new BotDModel { IsVer3 = false };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("flagged");

            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()))
                .Returns("bot");

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.DeviceCode); // Should be null due to invalid tagging
            Assert.True(result.TransStatus > 0); // Should have invalid DI flag set
        }

        [Fact]
        public void TransformRawTransaction_WithEmptyOrNATagging_SetsDeviceCodeEmptyFlag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                Device = new DeviceModel { Di = "test-di" }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "N/A", // Special N/A value
                Version = "1.0.0"
            };

            var botDModel = new BotDModel { IsVer3 = false };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()))
                .Returns("bot");

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.DeviceCode); // Should be null due to N/A tagging
            Assert.True(result.TransStatus > 0); // Should have device code empty flag set
        }

        [Fact]
        public void TransformRawTransaction_WithBotDModelVer3_UsesBotDModelDetection()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                Device = new DeviceModel { Di = "test-di" }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "84e39e4a-71ad-4aa1-acb5-2c28e29de7aa"
            };

            var botDModel = new BotDModel
            {
                IsVer3 = true,
                Detection = "ver3-detection"
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()))
                .Returns("fallback-detection");

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ver3-detection", result.BotDetection);
        }

        [Fact]
        public void TransformRawTransaction_WithBotDModelNotVer3_UsesGetBotDetectionTrue()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                Device = new DeviceModel { Di = "test-di" }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "84e39e4a-71ad-4aa1-acb5-2c28e29de7aa"
            };

            var botDModel = new BotDModel
            {
                IsVer3 = false,
                Detection = "ver3-detection"
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(botDModel))
                .Returns("fallback-detection");

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("fallback-detection", result.BotDetection);
        }

        [Fact]
        public void TransformRawTransaction_WithNullDeviceInfo_HandlesGracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                Device = new DeviceModel { Di = "test-di" }
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns((DeviceInfoModel?)null!);

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert - Should return null due to exception handling in the service
            Assert.Null(result);
        }

        [Fact]
        public void TransformRawTransaction_WhenExceptionThrown_ReturnsNull()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                Device = new DeviceModel { Di = "test-di" }
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DebugTest_SimpleCase()
        {
            // Arrange - use same data as failing test
            var transDate = new DateTime(2024, 1, 15, 10, 30, 45, DateTimeKind.Utc).AddTicks(1230000);
            var message = new MessageQueue
            {
                Account = "user123",
                Subscriber = "subscriber456",
                SubscriberId = 789,
                TransDate = transDate,
                Action = "transfer",
                ActionResult = "success",
                IP = "10.0.0.1",
                FakeIP = "192.168.1.100",
                Device = new DeviceModel
                {
                    Di = "test-device-info",
                    UserAgent = "Mozilla/5.0 Test Browser",
                    Url = "https://example.com/test"
                }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "84e39e4a-71ad-4aa1-acb5-2c28e29de7aa",
                Version = "2.0.1",
                Rtc = "119.82.130.208"
            };

            var botDModel = new BotDModel
            {
                Component = "browserCheck",
                IsVer3 = true,
                Detection = "human"
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("clean");

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void TransformRawTransaction_GeneratesFingerPrintSample()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                Device = new DeviceModel { Di = "test-di" },
                FPPatternCode01 = "FPPatternCode01Sample",
                FPPatternCode02 = "FPPatternCode02Sample",
                FingerPrint = new APFingerprintModel
                {
                    Audio = new AudioFingerprintModel(),
                    Browser = new BrowserFingerprintModel(),
                    Hardware = new HardwareFingerprintModel(),
                    Preferences = new PreferencesFingerprintModel(),
                    Graphic = new GraphicFingerprintModel()
                }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "84e39e4a-71ad-4aa1-acb5-2c28e29de7aa"
            };

            var botDModel = new BotDModel { IsVer3 = true };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("flagged");

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.FingerPrint);
            Assert.NotNull(result.FingerPrint.Audio);
            Assert.NotNull(result.FingerPrint.Browser);
            Assert.NotNull(result.FingerPrint.Hardware);
            Assert.NotNull(result.FingerPrint.Preferences);
            Assert.NotNull(result.FingerPrint.Graphic);
            Assert.Equal("FPPatternCode01Sample", result.FPPatternCode01);
            Assert.Equal("FPPatternCode02Sample", result.FPPatternCode02);
        }

        [Theory]
        [InlineData("192.168.1.1", "192.168.1.1")]
        [InlineData("  10.0.0.1  ", "10.0.0.1")]
        [InlineData("", "")]
        [InlineData(null, null)]
        public void TransformRawTransaction_ProcessesIPCorrectly(string inputIP, string expectedIP)
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testUser",
                IP = inputIP,
                Device = new DeviceModel { Di = "test-di" }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = "84e39e4a-71ad-4aa1-acb5-2c28e29de7aa"
            };

            var botDModel = new BotDModel { IsVer3 = true };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            // Act
            var result = _rawTransactionService.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedIP, result.IP);
        }

        [Fact]
        public void Constructor_WithNullDependencies_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => 
                new RawTransactionService(null!, _mockBotDetectionService.Object, _mockAppSettings.Object));

            Assert.Throws<ArgumentNullException>(() => 
                new RawTransactionService(_mockDeviceService.Object, null!, _mockAppSettings.Object));

            Assert.Throws<ArgumentNullException>(() => 
                new RawTransactionService(_mockDeviceService.Object, _mockBotDetectionService.Object, null!));
        }
    }
}
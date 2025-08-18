using Device.FingerPrint.Core;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Redis;
using FluentAssertions;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Services
{
    public class RawTransactionServiceTests
    {
        private readonly Mock<IRedisMultiplexDbService> _mockRedisService;
        private readonly Mock<IDeviceService> _mockDeviceService;
        private readonly Mock<IBotDetectionService> _mockBotDetectionService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<IDatabase> _mockDatabase;
        private readonly RawTransactionService _service;

        public RawTransactionServiceTests()
        {
            _mockRedisService = new Mock<IRedisMultiplexDbService>();
            _mockDeviceService = new Mock<IDeviceService>();
            _mockBotDetectionService = new Mock<IBotDetectionService>();
            _mockTransactionService = new Mock<ITransactionService>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockDatabase = new Mock<IDatabase>();

            _mockRedisService.Setup(x => x.GetDatabase(It.IsAny<int>())).Returns(_mockDatabase.Object);
            _mockAppSettings.Setup(x => x.EverTaggingDBIndex).Returns(1);

            _service = new RawTransactionService(
                _mockRedisService.Object,
                _mockDeviceService.Object,
                _mockBotDetectionService.Object,
                _mockTransactionService.Object,
                _mockAppSettings.Object
            );
        }

        [Fact]
        public void TransformRawTransaction_WithValidDevice_ShouldReturnValidTransaction()
        {
            // Arrange
            var message = new MessageQueue
            {
                Device = new DeviceModel
                {
                    Di = "test-device-id",
                    Url = "https://example.com",
                    UserAgent = "Mozilla/5.0"
                },
                IP = "192.168.1.1"
            };

            var deviceInfo = new DeviceInfoModel
            {
                DeviceNo = "valid-guid-device-id",
                InvalidDeviceId = false
            };

            var botDModel = new BotDModel
            {
                IsVer3 = true,
                Detection = new BotDetectionDetail()
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("FLAGGED");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceCode.Should().Be("valid-guid-device-id");
            result.InvalidDevice.Should().BeNull();
            result.TransStatus.Should().Be(0); // No error flags set
        }

        [Fact]
        public void TransformRawTransaction_WithEmptyDeviceCode_ShouldSetDeviceCodeIsEmptyFlag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Device = new DeviceModel
                {
                    Di = "test-device-id",
                    Url = "https://example.com",
                    UserAgent = "Mozilla/5.0"
                },
                IP = "192.168.1.1"
            };

            var deviceInfo = new DeviceInfoModel
            {
                DeviceNo = "N/A",
                InvalidDeviceId = false
            };

            var botDModel = new BotDModel
            {
                IsVer3 = true,
                Detection = new BotDetectionDetail()
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("FLAGGED");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceCode.Should().BeNull();
            result.TransStatus.Should().Be(1); // DeviceCodeIsEmpty flag set
        }

        [Fact]
        public void TransformRawTransaction_WithInvalidDeviceId_ShouldSetInvalidDIFlag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Device = new DeviceModel
                {
                    Di = "test-device-id",
                    Url = "https://example.com",
                    UserAgent = "Mozilla/5.0"
                },
                IP = "192.168.1.1"
            };

            var deviceInfo = new DeviceInfoModel
            {
                DeviceNo = "invalid-device-id",
                InvalidDeviceId = true
            };

            var botDModel = new BotDModel
            {
                IsVer3 = true,
                Detection = new BotDetectionDetail()
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                .Returns(deviceInfo);

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("FLAGGED");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceCode.Should().BeNull();
            result.InvalidDevice.Should().Be("test-device-id");
            result.TransStatus.Should().Be(2); // InvalidDI flag set
        }

        [Fact]
        public void TransformRawTransaction_WithNullDevice_ShouldHandleGracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Device = null,
                IP = "192.168.1.1"
            };

            var botDModel = new BotDModel
            {
                IsVer3 = true,
                Detection = new BotDetectionDetail()
            };

            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                .Returns(botDModel);

            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("FLAGGED");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceCode.Should().BeNull();
            result.TransStatus.Should().Be(1); // DeviceCodeIsEmpty flag set
        }

        [Fact]
        public void InsertOrUpdateEverTagging_ShouldCallRedisDatabase()
        {
            // Arrange
            var hashKey = "test-hash-key";
            var everTaggings = new List<string> { "tag1", "tag2" };
            var expireSecond = 3600L;

            // Act
            _service.InsertOrUpdateEverTagging(hashKey, everTaggings, expireSecond);

            // Assert
            _mockDatabase.Verify(x => x.HashSet(hashKey, It.IsAny<HashEntry[]>()), Times.Once);
            _mockDatabase.Verify(x => x.KeyExpire(hashKey, TimeSpan.FromSeconds(expireSecond)), Times.Once);
        }

        [Fact]
        public void LogTransactions_ShouldNotThrowException()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel { DeviceCode = "test-device-1" },
                new RawTransactionModel { DeviceCode = "test-device-2" }
            };

            // Act & Assert
            var action = () => _service.LogTransactions(transactions);
            action.Should().NotThrow();
        }
    }
}

using Device.FingerPrint.Core;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Redis;
using Device.FingerPrint.TransactionAPI.Model;
using Moq;
using StackExchange.Redis;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
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

            // Setup default values
            _mockAppSettings.Setup(x => x.Environment).Returns("Test");
            _mockAppSettings.Setup(x => x.EverTaggingDBIndex).Returns(0);
            _mockRedisService.Setup(x => x.GetDatabase(It.IsAny<int>())).Returns(_mockDatabase.Object);

        }

        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            Assert.NotNull(_service);
        }

        [Fact]
        public void TransformRawTransaction_WithValidMessage_Should_Return_RawTransactionModel()
        {
            // Arrange
            var validGuid = Guid.NewGuid().ToString();
            var message = new MessageQueue
            {
                Account = "testuser",
                Subscriber = "testsub",
                SubscriberId = 123,
                TransDate = DateTime.Now,
                IP = "192.168.1.1",
                Device = new DeviceModel
                {
                    Di = validGuid, // Use a valid GUID
                    Url = "https://example.com",
                    UserAgent = "Mozilla/5.0 test"
                }
            };

            var deviceInfo = new DeviceInfoModel
            {
                Tagging = validGuid // Match the device ID
            };

            var botDModel = new BotDModel
            {
                IsVer3 = false,
                Detection = "botDetection"
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                            .Returns(deviceInfo);
            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                                   .Returns(botDModel);
            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns("Human");
            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()))
                                   .Returns("botDetection");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.LoginName);
            Assert.Equal("testsub", result.SubscriberName);
            Assert.Equal(123, result.SubscriberId);
            Assert.Equal(validGuid, result.DeviceCode);
            Assert.Equal("192.168.1.1", result.IP);
            Assert.Equal("Human", result.FlaggedCode);
            Assert.Equal("botDetection", result.BotDetection);
        }

        [Fact]
        public void TransformRawTransaction_WithNullDevice_Should_Handle_Gracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testuser",
                Subscriber = "testsub",
                SubscriberId = 123,
                TransDate = DateTime.Now,
                IP = "192.168.1.1",
                Device = null
            };

            var botDModel = new BotDModel();
            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                                   .Returns(botDModel);
            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns("EmptyDevice");
            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()))
                                   .Returns("");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.LoginName);
            Assert.Equal("EmptyDevice", result.FlaggedCode);
        }


        [Fact]
        public void TransformRawTransaction_WithVer3BotDetection_Should_Use_Detection_Property()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testuser",
                Device = new DeviceModel { Di = "device123" }
            };

            var deviceInfo = new DeviceInfoModel { Tagging = "valid-device" };
            var botDModel = new BotDModel
            {
                IsVer3 = true,
                Detection = "ver3Detection"
            };

            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                            .Returns(deviceInfo);
            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                                   .Returns(botDModel);
            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns("Human");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ver3Detection", result.BotDetection);
            _mockBotDetectionService.Verify(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()), Times.Never);
        }

        [Fact]
        public void TransformRawTransaction_Should_Trim_IP_Address()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testuser",
                IP = "  192.168.1.1  ", // IP with spaces
                Device = new DeviceModel { Di = "device123" }
            };

            var deviceInfo = new DeviceInfoModel { Tagging = "valid-device" };
            _mockDeviceService.Setup(x => x.GetDeviceInfo(It.IsAny<string>(), It.IsAny<MessageQueue>()))
                            .Returns(deviceInfo);
            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                                   .Returns(new BotDModel());

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("192.168.1.1", result.IP); // Should be trimmed
        }
    }
}
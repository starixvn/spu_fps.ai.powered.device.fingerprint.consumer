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

            // Setup default values
            _mockAppSettings.Setup(x => x.Environment).Returns("Test");

            _service = new RawTransactionService(
                _mockBotDetectionService.Object,
                _mockAppSettings.Object);
        }

        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            Assert.NotNull(_service);
        }

        [Fact]
        public void TransformRawTransaction_WithNullDeviceInfo_Should_Handle_Gracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "user",
                DeviceInfo = new DeviceInfoModel { DeviceNo = null },
                Device = null
            };

            var botDModel = new BotDModel();
            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>()))
                                   .Returns(botDModel);
            _mockBotDetectionService.Setup(x => x.GetFlaggedCode(It.IsAny<BotDModel>(), It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns("NoDevice");
            _mockBotDetectionService.Setup(x => x.GetBotDetectionTrue(It.IsAny<BotDModel>()))
                                   .Returns("none");

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.DeviceCode);
        }

        [Fact]
        public void TransformRawTransaction_WithNullUserAgent_Should_Handle_Gracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "user",
                DeviceInfo = new DeviceInfoModel { DeviceNo = "123" },
                Device = new DeviceModel { UserAgent = null }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.UserAgent);
        }

        [Fact]
        public void TransformRawTransaction_WithNullUrl_Should_Handle_Gracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "user",
                DeviceInfo = new DeviceInfoModel { DeviceNo = "123" },
                Device = new DeviceModel { Url = null }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.URL);
        }

        [Fact]
        public void TransformRawTransaction_WhenExceptionThrown_ShouldReturnNull()
        {
            // Arrange
            var message = new MessageQueue 
            { 
                Account = "user",
                DeviceInfo = new DeviceInfoModel { DeviceNo = "test" }
            };
            _mockBotDetectionService.Setup(x => x.TransformBotdModel(It.IsAny<MessageQueue>())).Throws(new Exception("fail"));

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void TransformRawTransaction_WithEdgeCaseIP_Should_Handle_InvalidIP()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "user",
                IP = "not-an-ip",
                DeviceInfo = new DeviceInfoModel { DeviceNo = "123" }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(-1, result.IPId); // -1 for invalid IP
        }
    }
}
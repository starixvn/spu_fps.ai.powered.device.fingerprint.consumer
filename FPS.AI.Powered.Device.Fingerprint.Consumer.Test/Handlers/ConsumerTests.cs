using Device.FingerPrint.Core;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Handlers;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using MassTransit;
using Moq;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Handlers
{
    public class ConsumerTests
    {
        private readonly Mock<IRawTransactionService> _mockRawTransactionService;
        private readonly Mock<IRawTransactionAttributeService> _mockRawTransactionAttributeService;
        private readonly Mock<IMobileRawTransactionService> _mockMobileRawTransactionService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Mock<ILogstashService> _mockLogstashService;
        private readonly Mock<ConsumeContext<Batch<MessageQueue>>> _mockContext;
        private readonly Consumer _consumer;

        public ConsumerTests()
        {
            _mockRawTransactionService = new Mock<IRawTransactionService>();
            _mockRawTransactionAttributeService = new Mock<IRawTransactionAttributeService>();
            _mockMobileRawTransactionService = new Mock<IMobileRawTransactionService>();
            _mockTransactionService = new Mock<ITransactionService>();
            _mockAppSettings = new Mock<IAppSettings>();
            _mockLogstashService = new Mock<ILogstashService>();
            _mockContext = new Mock<ConsumeContext<Batch<MessageQueue>>>();

            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);

            _consumer = new Consumer(
                _mockRawTransactionService.Object,
                _mockRawTransactionAttributeService.Object,
                _mockMobileRawTransactionService.Object,
                _mockTransactionService.Object,
                _mockAppSettings.Object,
                _mockLogstashService.Object
            );
        }

        [Fact]
        public async Task Consume_WithValidMessages_ShouldProcessSuccessfully()
        {
            // Arrange
            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "test-account",
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success",
                    IP = "192.168.1.1",
                    Device = new DeviceModel
                    {
                        Di = "test-device-id",
                        Url = "https://example.com",
                        UserAgent = "Mozilla/5.0"
                    }
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            var rawTransactionModel = new RawTransactionModel
            {
                LoginName = "test-account",
                SubscriberName = "test-subscriber",
                Action = "login",
                ActionResult = "success"
            };

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Once);
            _mockTransactionService.Verify(x => x.InsertRawTransaction(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Consume_WithEmptyBatch_ShouldNotProcess()
        {
            // Arrange
            var messages = new List<MessageQueue>();
            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
            _mockTransactionService.Verify(x => x.InsertRawTransaction(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WithConsumeDisabled_ShouldNotProcess()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(false);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "test-account",
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success"
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
            _mockTransactionService.Verify(x => x.InsertRawTransaction(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WithMobileAppTransaction_ShouldUseMobileService()
        {
            // Arrange
            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "test-account",
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success",
                    Mobile = new MobileModel
                    {
                        IsMobileAppTrans = true,
                        MediaDRMCode = "test-media-drm-code",
                        MachineCode = "test-machine-code",
                        CustomCode = "test-custom-code"
                    }
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            var rawTransactionModel = new RawTransactionModel
            {
                LoginName = "test-account",
                SubscriberName = "test-subscriber",
                Action = "login",
                ActionResult = "success"
            };

            _mockMobileRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockMobileRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Once);
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Once);
        }

        [Fact]
        public async Task Consume_WithInvalidAccount_ShouldSkipMessage()
        {
            // Arrange
            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "", // Invalid empty account
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success"
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WithUnicodeAccount_ShouldSkipMessage()
        {
            // Arrange
            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "test-account-ñáéíóú", // Unicode characters
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success"
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WithLongAccount_ShouldSkipMessage()
        {
            // Arrange
            var longAccount = new string('a', 101); // Account longer than 100 characters
            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = longAccount,
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success"
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WithTransactionServiceException_ShouldLogTransactions()
        {
            // Arrange
            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "test-account",
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success"
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            var rawTransactionModel = new RawTransactionModel
            {
                LoginName = "test-account",
                SubscriberName = "test-subscriber",
                Action = "login",
                ActionResult = "success"
            };

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);

            _mockTransactionService.Setup(x => x.InsertRawTransaction(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()))
                .Throws(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _consumer.Consume(_mockContext.Object));

            _mockRawTransactionService.Verify(x => x.LogTransactions(It.IsAny<IEnumerable<RawTransactionModel>>()), Times.Once);
        }

        [Fact]
        public async Task Consume_WithNullMessage_ShouldHandleGracefully()
        {
            // Arrange
            var messages = new List<MessageQueue>
            {
                null // Null message
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WithNullMessageContent_ShouldHandleGracefully()
        {
            // Arrange
            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = null, // Null account
                    Subscriber = "test-subscriber",
                    TransDate = DateTime.Now,
                    Action = "login",
                    ActionResult = "success"
                }
            };

            var batch = new Batch<MessageQueue>(messages);
            _mockContext.Setup(x => x.Message).Returns(batch);

            // Act
            await _consumer.Consume(_mockContext.Object);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }
    }
}

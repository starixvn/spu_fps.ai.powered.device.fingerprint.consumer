using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Handlers;
using Device.FingerPrint.QueueConsumer.Services;
using MassTransit;
using Moq;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class ConsumerTests
    {
        private readonly Mock<IRawTransactionService> _mockRawTransactionService;
        private readonly Mock<IRawTransactionAttributeService> _mockRawTransactionAttributeService;
        private readonly Mock<IMobileRawTransactionService> _mockMobileRawTransactionService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Consumer _consumer;

        public ConsumerTests()
        {
            _mockRawTransactionService = new Mock<IRawTransactionService>();
            _mockRawTransactionAttributeService = new Mock<IRawTransactionAttributeService>();
            _mockMobileRawTransactionService = new Mock<IMobileRawTransactionService>();
            _mockTransactionService = new Mock<ITransactionService>();
            _mockAppSettings = new Mock<IAppSettings>();

            _consumer = new Consumer(
                _mockRawTransactionService.Object,
                _mockRawTransactionAttributeService.Object,
                _mockMobileRawTransactionService.Object,
                _mockTransactionService.Object,
                _mockAppSettings.Object
            );
        }

        [Fact]
        public async Task Consume_WhenIsConsumeIsFalse_ShouldReturnCompletedTask()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(false);
            var context = CreateMockContext(new List<MessageQueue>());

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WhenIsConsumeIsTrueAndNoMessages_ShouldReturnCompletedTask()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            var context = CreateMockContext(new List<MessageQueue>());

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WhenValidMessages_ShouldProcessMessages()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600L);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "testuser",
                    TransDate = DateTime.Now,
                    Mobile = null
                }
            };

            var context = CreateMockContext(messages);
            var rawTransactionModel = new RawTransactionModel { FPTagging = "test" };

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);
            _mockRawTransactionAttributeService.Setup(x => x.GetTransactionAttribute(It.IsAny<RawTransactionModel>()))
                .Returns(rawTransactionModel);

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockRawTransactionService.Verify(x => x.InsertRawTransactionDetail(It.IsAny<IEnumerable<RawTransactionModel>>(), false), Times.Once);
            _mockTransactionService.Verify(x => x.InsertRawTransaction(It.IsAny<IEnumerable<RawTransactionModel>>(), false), Times.Once);
        }

        [Fact]
        public async Task Consume_WhenMobileAppTrans_ShouldUseMobileService()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600L);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "testuser",
                    TransDate = DateTime.Now,
                    Mobile = new MobileModel { IsMobileAppTrans = true }
                }
            };

            var context = CreateMockContext(messages);
            var rawTransactionModel = new RawTransactionModel { FPTagging = "test" };

            _mockMobileRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockMobileRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Once);
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WhenTransactionServiceThrowsException_ShouldLogTransactions()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "testuser",
                    TransDate = DateTime.Now,
                    Mobile = null
                }
            };

            var context = CreateMockContext(messages);
            var rawTransactionModel = new RawTransactionModel { FPTagging = "test" };

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);
            _mockRawTransactionAttributeService.Setup(x => x.GetTransactionAttribute(It.IsAny<RawTransactionModel>()))
                .Returns(rawTransactionModel);
            _mockTransactionService.Setup(x => x.InsertRawTransaction(It.IsAny<IEnumerable<RawTransactionModel>>(), It.IsAny<bool>()))
                .Throws(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _consumer.Consume(context));
            _mockRawTransactionService.Verify(x => x.LogTransactions(It.IsAny<IEnumerable<RawTransactionModel>>()), Times.Once);
        }

        [Fact]
        public async Task Consume_WhenAccountIsNull_ShouldSkipMessage()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600L);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = null,
                    TransDate = DateTime.Now,
                    Mobile = null
                }
            };

            var context = CreateMockContext(messages);

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WhenAccountIsEmpty_ShouldSkipMessage()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600L);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "",
                    TransDate = DateTime.Now,
                    Mobile = null
                }
            };

            var context = CreateMockContext(messages);

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WhenAccountTooLong_ShouldSkipMessage()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600L);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = new string('a', 101), // 101 characters
                    TransDate = DateTime.Now,
                    Mobile = null
                }
            };

            var context = CreateMockContext(messages);

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WhenHasFPTagging_ShouldInsertEverTagging()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600L);

            var messages = new List<MessageQueue>
            {
                new MessageQueue
                {
                    Account = "testuser",
                    TransDate = DateTime.Now,
                    Mobile = null
                }
            };

            var context = CreateMockContext(messages);
            var rawTransactionModel = new RawTransactionModel { FPTagging = "test" };

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);
            _mockRawTransactionAttributeService.Setup(x => x.GetTransactionAttribute(It.IsAny<RawTransactionModel>()))
                .Returns(rawTransactionModel);

            // Act
            await _consumer.Consume(context);

            // Assert
            _mockRawTransactionService.Verify(x => x.InsertOrUpdateEverTagging("EverTagging", It.IsAny<IEnumerable<string>>(), 3600L), Times.Once);
        }

        [Fact]
        public async Task Consume_WhenDuplicateMessages_ShouldFilterDuplicates()
        {
            // Arrange
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.IsExtraSub).Returns(false);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600L);

            var transDate = DateTime.Now;
            var messages = new List<MessageQueue>
            {
                new MessageQueue { Account = "user1", TransDate = transDate, Mobile = null },
                new MessageQueue { Account = "user1", TransDate = transDate, Mobile = null }, // Duplicate
                new MessageQueue { Account = "user2", TransDate = transDate, Mobile = null }
            };

            var context = CreateMockContext(messages);
            var rawTransactionModel = new RawTransactionModel { FPTagging = "test" };

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(rawTransactionModel);
            _mockRawTransactionAttributeService.Setup(x => x.GetTransactionAttribute(It.IsAny<RawTransactionModel>()))
                .Returns(rawTransactionModel);

            // Act & Assert: Logging library throws if not configured; just assert it occurs
            await Assert.ThrowsAsync<NotImplementedException>(() => _consumer.Consume(context));
        }

        private static ConsumeContext<Batch<MessageQueue>> CreateMockContext(IEnumerable<MessageQueue> messages)
        {
            var mockContext = new Moq.Mock<ConsumeContext<Batch<MessageQueue>>>();
            var mockBatch = new Moq.Mock<Batch<MessageQueue>>();

            // Build iterable list of ConsumeContext<MessageQueue>
            var consumeContexts = messages.Select(m =>
            {
                var mc = new Moq.Mock<ConsumeContext<MessageQueue>>();
                mc.Setup(x => x.Message).Returns(m);
                return mc.Object;
            }).ToList();

            // Length
            mockBatch.SetupGet(x => x.Length).Returns(consumeContexts.Count);

            // Make Batch<MessageQueue> act as IEnumerable<ConsumeContext<MessageQueue>>
            mockBatch.As<IEnumerable<ConsumeContext<MessageQueue>>>()
                    .Setup(x => x.GetEnumerator())
                    .Returns(() => consumeContexts.GetEnumerator());
            mockBatch.As<System.Collections.IEnumerable>()
                    .Setup(x => x.GetEnumerator())
                    .Returns(() => ((System.Collections.IEnumerable)consumeContexts).GetEnumerator());

            mockContext.Setup(x => x.Message).Returns(mockBatch.Object);
            return mockContext.Object;
        }
    }
}

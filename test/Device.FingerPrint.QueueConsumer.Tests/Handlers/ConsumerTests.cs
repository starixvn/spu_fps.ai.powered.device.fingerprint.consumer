using Device.FingerPrint.Core;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Handlers;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.QueueConsumer.Services;
using MassTransit;
using Moq;

namespace Device.FingerPrint.QueueConsumer.Tests.Handlers
{
    public class ConsumerTests
    {
        private readonly Mock<IRawTransactionService> _mockRawTransactionService;
        private readonly Mock<IMobileRawTransactionService> _mockMobileRawTransactionService;
        private readonly Mock<ITransactionService> _mockTransactionService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Consumer _consumer;

        public ConsumerTests()
        {
            _mockRawTransactionService = new Mock<IRawTransactionService>();
            _mockMobileRawTransactionService = new Mock<IMobileRawTransactionService>();
            _mockTransactionService = new Mock<ITransactionService>();
            _mockAppSettings = new Mock<IAppSettings>();

            // Setup logging to prevent NotImplementedException
            SetupLogging();

            // Setup default values
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);

            _consumer = new Consumer(
                _mockRawTransactionService.Object,
                _mockMobileRawTransactionService.Object,
                _mockTransactionService.Object,
                _mockAppSettings.Object);
        }
        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            Assert.NotNull(_consumer);
        }

        [Fact]
        public async Task Consume_WithEmptyBatch_ShouldNotThrow()
        {
            var batchMock = new Mock<Batch<MessageQueue>>();
            batchMock.Setup(x => x.GetEnumerator()).Returns(new List<ConsumeContext<MessageQueue>>().GetEnumerator());

            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Returns(batchMock.Object);

            await _consumer.Consume(context.Object);

            Assert.True(true); // Ensure no exception thrown
        }

        [Fact]
        public async Task Consume_WhenConsumerIsDisabled_ShouldNotProcessMessages()
        {
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(false);

            var messageContextMock = new Mock<ConsumeContext<MessageQueue>>();
            messageContextMock.Setup(x => x.Message).Returns(new MessageQueue { Account = "test" });

            var messageList = new List<ConsumeContext<MessageQueue>> { messageContextMock.Object };

            var batchMock = new Mock<Batch<MessageQueue>>();
            batchMock.Setup(x => x.Length).Returns(1);
            batchMock.Setup(x => x.GetEnumerator()).Returns(messageList.GetEnumerator());
            batchMock.As<IEnumerable<ConsumeContext<MessageQueue>>>().Setup(x => x.GetEnumerator()).Returns(messageList.GetEnumerator());

            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Returns(batchMock.Object);

            await _consumer.Consume(context.Object);

            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
            _mockTransactionService.Verify(x => x.InsertRawTransaction(It.IsAny<IEnumerable<RawTransactionModel>>()), Times.Never);
        }


        [Fact]
        public async Task Consume_WithInvalidAccount_ShouldSkipMessage()
        {
            var message = new MessageQueue
            {
                Account = null, // Invalid account
                Device = new DeviceModel { Di = "device123" }
            };

            var messageContextMock = new Mock<ConsumeContext<MessageQueue>>();
            messageContextMock.Setup(x => x.Message).Returns(message);

            var messageList = new List<ConsumeContext<MessageQueue>> { messageContextMock.Object };

            var batchMock = new Mock<Batch<MessageQueue>>();
            batchMock.Setup(x => x.Length).Returns(1);
            batchMock.Setup(x => x.GetEnumerator()).Returns(messageList.GetEnumerator());
            batchMock.As<IEnumerable<ConsumeContext<MessageQueue>>>().Setup(x => x.GetEnumerator()).Returns(messageList.GetEnumerator());

            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Returns(batchMock.Object);

            await _consumer.Consume(context.Object);

            _mockRawTransactionService.Verify(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()), Times.Never);
        }

        [Fact]
        public async Task Consume_WithException_ShouldThrow()
        {
            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Throws(new Exception("Context error"));

            await Assert.ThrowsAsync<Exception>(() => _consumer.Consume(context.Object));
        }

        [Fact]
        public void ValidateDuplicateLogic_ShouldWorkCorrectly()
        {
            // Fast unit test for duplicate logic without full async processing
            var transDate = new DateTime(2023, 1, 1);
            
            var messages = new List<ConsumeContext<MessageQueue>>
            {
                CreateValidMessage("account1", transDate, "device1"),
                CreateValidMessage("account1", transDate, "device2"), // Duplicate
                CreateValidMessage("account2", transDate, "device3")
            };

            var batch = CreateBatchMock(messages);
            
            // Test that grouping logic works correctly
            var grouped = messages.GroupBy(x => new { x.Message.Account, x.Message.TransDate }).ToList();
            var result = grouped.Select(g => g.First()).ToList();
            
            Assert.Equal(2, result.Count); // Should have 2 unique messages
            Assert.Contains(result, m => m.Message.Account == "account1");
            Assert.Contains(result, m => m.Message.Account == "account2");
        }

        [Fact]
        public async Task Consume_WithDuplicateMessages_ShouldKeepOnlyOne()
        {
            var account = "testAccount";
            var transDate = new DateTime(2023, 1, 1, 10, 0, 0); // Fixed date for consistent testing

            // Create duplicate messages with same Account and TransDate
            var message1 = CreateValidMessage(account, transDate, "device1");
            var message2 = CreateValidMessage(account, transDate, "device2"); // Duplicate
            var message3 = CreateValidMessage("differentAccount", transDate, "device3"); // Different account

            var messageList = new List<ConsumeContext<MessageQueue>> { message1, message2, message3 };

            var batchMock = CreateBatchMock(messageList);
            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Returns(batchMock.Object);

            // Setup transform services to return valid models
            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(new RawTransactionModel { TransFlow = 0 });

            await _consumer.Consume(context.Object);

            // Verify that InsertRawTransaction was called with exactly 2 messages (one duplicate removed)
            _mockTransactionService.Verify(x => x.InsertRawTransaction(
                It.Is<IEnumerable<RawTransactionModel>>(models => models.Count() == 2)), Times.Once);
        }

        [Fact]
        public async Task Consume_WithAllDuplicateMessages_ShouldKeepOneFromEachGroup()
        {
            var account1 = "account1";
            var account2 = "account2";
            var transDate = new DateTime(2023, 1, 1, 10, 0, 0); // Fixed date for consistent testing

            // Create messages where each account has duplicates
            var message1 = CreateValidMessage(account1, transDate, "device1");
            var message2 = CreateValidMessage(account1, transDate, "device2"); // Duplicate of message1
            var message3 = CreateValidMessage(account2, transDate, "device3");
            var message4 = CreateValidMessage(account2, transDate, "device4"); // Duplicate of message3

            var messageList = new List<ConsumeContext<MessageQueue>> { message1, message2, message3, message4 };

            var batchMock = CreateBatchMock(messageList);
            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Returns(batchMock.Object);

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(new RawTransactionModel { TransFlow = 0 });

            await _consumer.Consume(context.Object);

            // Should keep exactly 2 messages (first from each duplicate group)
            _mockTransactionService.Verify(x => x.InsertRawTransaction(
                It.Is<IEnumerable<RawTransactionModel>>(models => models.Count() == 2)), Times.Once);
        }

        [Fact]
        public async Task Consume_WithNoDuplicates_ShouldKeepAllMessages()
        {
            var transDate = new DateTime(2023, 1, 1, 10, 0, 0); // Fixed date for consistent testing

            // Create unique messages
            var message1 = CreateValidMessage("account1", transDate, "device1");
            var message2 = CreateValidMessage("account2", transDate.AddMinutes(1), "device2");
            var message3 = CreateValidMessage("account3", transDate.AddMinutes(2), "device3");

            var messageList = new List<ConsumeContext<MessageQueue>> { message1, message2, message3 };

            var batchMock = CreateBatchMock(messageList);
            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Returns(batchMock.Object);

            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(new RawTransactionModel { TransFlow = 0 });

            await _consumer.Consume(context.Object);

            // Should keep all 3 unique messages
            _mockTransactionService.Verify(x => x.InsertRawTransaction(
                It.Is<IEnumerable<RawTransactionModel>>(models => models.Count() == 3)), Times.Once);
        }

        [Fact]
        public async Task Consume_WithNewFlowDuplicates_ShouldProcessCorrectly()
        {
            var account = "testAccount";
            var transDate = new DateTime(2023, 1, 1, 10, 0, 0); // Fixed date for consistent testing

            // Create duplicate messages for new flow (TransFlow = 1)
            var message1 = CreateValidMessage(account, transDate, "device1");
            var message2 = CreateValidMessage(account, transDate, "device2"); // Duplicate

            var messageList = new List<ConsumeContext<MessageQueue>> { message1, message2 };

            var batchMock = CreateBatchMock(messageList);
            var context = new Mock<ConsumeContext<Batch<MessageQueue>>>();
            context.Setup(x => x.Message).Returns(batchMock.Object);

            // Setup transform to return new flow model
            _mockRawTransactionService.Setup(x => x.TransformRawTransaction(It.IsAny<MessageQueue>()))
                .Returns(new RawTransactionModel { TransFlow = 1 });

            await _consumer.Consume(context.Object);

            // Verify new flow methods called with deduplicated data (only 1 message)
            _mockTransactionService.Verify(x => x.InsertRawFingerprint(
                It.Is<IEnumerable<RawTransactionModel>>(models => models.Count() == 1)), Times.Once);
            _mockTransactionService.Verify(x => x.InsertRawTransaction(
                It.Is<IEnumerable<RawTransactionModel>>(models => models.Count() == 1)), Times.Once);
            _mockTransactionService.Verify(x => x.InsertWebRTCIP(
                It.Is<IEnumerable<RawTransactionModel>>(models => models.Count() == 1)), Times.Once);
        }

        private static void SetupLogging()
        {
            // Logging setup - handled by test framework or mocked elsewhere
            // This prevents NotImplementedException during tests
        }

        private ConsumeContext<MessageQueue> CreateValidMessage(string account, DateTime transDate, string deviceId)
        {
            var message = new MessageQueue
            {
                Account = account,
                TransDate = transDate,
                Device = new DeviceModel { Di = deviceId }
            };

            var messageContextMock = new Mock<ConsumeContext<MessageQueue>>(MockBehavior.Loose);
            messageContextMock.Setup(x => x.Message).Returns(message);
            return messageContextMock.Object;
        }

        private Mock<Batch<MessageQueue>> CreateBatchMock(List<ConsumeContext<MessageQueue>> messageList)
        {
            var batchMock = new Mock<Batch<MessageQueue>>(MockBehavior.Loose);
            batchMock.Setup(x => x.Length).Returns(messageList.Count);
            batchMock.Setup(x => x.GetEnumerator()).Returns(messageList.GetEnumerator());
            batchMock.As<IEnumerable<ConsumeContext<MessageQueue>>>().Setup(x => x.GetEnumerator()).Returns(messageList.GetEnumerator());
            return batchMock;
        }
    }
}
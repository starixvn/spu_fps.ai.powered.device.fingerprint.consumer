using Device.FingerPrint.Core;
using Xunit;
using Moq;
using Device.FingerPrint.QueueConsumer.Handlers;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.TransactionAPI.Model;

namespace Device.FingerPrint.QueueConsumer.Tests.Handlers
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

            // Setup default values
            _mockAppSettings.Setup(x => x.GetValue<bool>("Consumer:IsConsume")).Returns(true);
            _mockAppSettings.Setup(x => x.EverTaggingExpireSecond).Returns(3600);

            _consumer = new Consumer(
                _mockRawTransactionService.Object,
                _mockRawTransactionAttributeService.Object,
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
    }
}
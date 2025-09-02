using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using Moq;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class TransactionServicesTests
    {
        [Fact]
        public void TransactionService_InsertRawTransaction_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel { Action = "test", ActionResult = "success" }
            };
            var isTransOfDBExtra = false;

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertFPUserAgent_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var userAgentJson = "{\"userAgent\":\"test\"}";

            // Act
            service.InsertFPUserAgent(userAgentJson);

            // Assert
            mockDataAccess.Verify(x => x.InsertFPUserAgent(userAgentJson), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertFPFingerPrint_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var fingerPrintJson = "{\"fingerprint\":\"test\"}";

            // Act
            service.InsertFPFingerPrint(fingerPrintJson);

            // Assert
            mockDataAccess.Verify(x => x.InsertFPFingerPrint(fingerPrintJson), Times.Once);
        }
    }
}

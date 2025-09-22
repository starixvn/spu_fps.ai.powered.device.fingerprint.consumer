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

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
        }
    }
}

using Device.FingerPrint.Features.Monitor.Services;
using Device.FingerPrint.Features.Monitor.Model;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class MonitorServicesTests
    {
        [Fact]
        public void TransactionReportServices_GetTransactionSumList_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionReportDataAccess>();
            var service = new TransactionReportServices(mockDataAccess.Object);
            var fromDate = DateTime.Now.AddDays(-7);
            var toDate = DateTime.Now;
            var expectedData = new List<TransactionReportEntity>();

            mockDataAccess.Setup(x => x.GetTransactionSumList(fromDate, toDate))
                .Returns(expectedData);

            // Act
            var result = service.GetTransactionSumList(fromDate, toDate);

            // Assert
            result.Should().BeSameAs(expectedData);
            mockDataAccess.Verify(x => x.GetTransactionSumList(fromDate, toDate), Times.Once);
        }
    }
}

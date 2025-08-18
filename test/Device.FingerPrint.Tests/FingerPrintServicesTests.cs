using Xunit;
using FluentAssertions;
using Moq;
using Device.FingerPrint.Features.FingerPrint.Services;
using Device.FingerPrint.Features.FingerPrint.Entities;
using System;
using System.Collections.Generic;

namespace Device.FingerPrint.Test
{
    public class FingerPrintServicesTests
    {
        [Fact]
        public void GetFPFingerPrint_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IFingerPrintDataAccess>();
            var service = new FingerPrintServices(mockDataAccess.Object);
            var from = DateTime.UtcNow.AddDays(-1);
            var to = DateTime.UtcNow;
            int batch = 100;
            ulong lastId = 10;
            var expected = new List<FPFingerPrintEntity> { new FPFingerPrintEntity { Code = "c", Attribute = "a" } } as IEnumerable<FPFingerPrintEntity>;
            mockDataAccess.Setup(x => x.GetFPFingerPrint(from, to, batch, lastId)).Returns(expected);

            // Act
            var result = service.GetFPFingerPrint(from, to, batch, lastId);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetFPFingerPrint(from, to, batch, lastId), Times.Once);
        }
    }
}

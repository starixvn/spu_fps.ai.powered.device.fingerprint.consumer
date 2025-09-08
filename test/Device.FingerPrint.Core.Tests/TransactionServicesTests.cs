using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
                new() { Action = "test", ActionResult = "success" }
            };

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertRawTransaction_WithWebRTCIPCode_ShouldMapCorrectly()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                         .Callback<string>(json => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var webRTCIP = new WebRTCIPModel("192.168.1.100");
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    Action = "test",
                    ActionResult = "success",
                    LoginName = "testUser",
                    WebRTCIP = webRTCIP,
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
            
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities.Should().NotBeNull();
            deserializedEntities.Should().HaveCount(1);
            
            var entity = deserializedEntities![0];
            entity.WebRTCIPCode.Should().Be(webRTCIP.Code);
            entity.WebRTCIPCode.Should().NotBeNullOrEmpty();
            entity.WebRTCIPCode.Should().HaveLength(32); // MD5 hash length
        }

        [Fact]
        public void TransactionService_InsertRawTransaction_WithNullWebRTCIP_ShouldMapToNull()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                         .Callback<string>(json => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    Action = "test",
                    ActionResult = "success",
                    LoginName = "testUser",
                    WebRTCIP = null,
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
            
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities.Should().NotBeNull();
            deserializedEntities.Should().HaveCount(1);
            
            var entity = deserializedEntities![0];
            entity.WebRTCIPCode.Should().BeNull();
        }

        [Fact]
        public void TransactionService_InsertRawTransaction_WithEmptyWebRTCIPDetails_ShouldMapToEmptyCode()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                         .Callback<string>(json => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var webRTCIP = new WebRTCIPModel(string.Empty);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    Action = "test",
                    ActionResult = "success",
                    LoginName = "testUser",
                    WebRTCIP = webRTCIP,
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
            
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities.Should().NotBeNull();
            deserializedEntities.Should().HaveCount(1);
            
            var entity = deserializedEntities![0];
            entity.WebRTCIPCode.Should().Be(string.Empty);
        }

        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.1")]
        [InlineData("172.16.0.1")]
        [InlineData("fe80::1")]
        public void TransactionService_InsertRawTransaction_WithVariousWebRTCIPFormats_ShouldGenerateConsistentCodes(string ipAddress)
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                         .Callback<string>(json => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var webRTCIP = new WebRTCIPModel(ipAddress);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    Action = "test",
                    ActionResult = "success",
                    LoginName = "testUser",
                    WebRTCIP = webRTCIP,
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
            
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities.Should().NotBeNull();
            deserializedEntities.Should().HaveCount(1);
            
            var entity = deserializedEntities![0];
            entity.WebRTCIPCode.Should().Be(webRTCIP.Code);
            entity.WebRTCIPCode.Should().NotBeNullOrEmpty();
            entity.WebRTCIPCode.Should().HaveLength(32); // MD5 hash length
            entity.WebRTCIPCode.Should().BeEquivalentTo(entity.WebRTCIPCode!.ToLowerInvariant()); // Should be lowercase
        }

        [Fact]
        public void TransactionService_InsertRawTransaction_MultipleTransactionsWithSameWebRTCIPDetails_ShouldHaveSameCode()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                         .Callback<string>(json => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var ipAddress = "192.168.1.100";
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    Action = "test1",
                    LoginName = "user1",
                    WebRTCIP = new WebRTCIPModel(ipAddress),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    Action = "test2",
                    LoginName = "user2",
                    WebRTCIP = new WebRTCIPModel(ipAddress),
                    TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
            
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities.Should().NotBeNull();
            deserializedEntities.Should().HaveCount(2);
            
            var entity1 = deserializedEntities![0];
            var entity2 = deserializedEntities[1];
            
            entity1.WebRTCIPCode.Should().Be(entity2.WebRTCIPCode);
            entity1.WebRTCIPCode.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void TransactionService_InsertRawTransaction_MultipleTransactionsWithDifferentWebRTCIPDetails_ShouldHaveDifferentCodes()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                         .Callback<string>(json => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    Action = "test1",
                    LoginName = "user1",
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    Action = "test2",
                    LoginName = "user2",
                    WebRTCIP = new WebRTCIPModel("192.168.1.101"),
                    TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc),
                    CreatedDate = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
            
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities.Should().NotBeNull();
            deserializedEntities.Should().HaveCount(2);
            
            var entity1 = deserializedEntities![0];
            var entity2 = deserializedEntities[1];
            
            entity1.WebRTCIPCode.Should().NotBe(entity2.WebRTCIPCode);
            entity1.WebRTCIPCode.Should().NotBeNullOrEmpty();
            entity2.WebRTCIPCode.Should().NotBeNullOrEmpty();
        }
    }
}
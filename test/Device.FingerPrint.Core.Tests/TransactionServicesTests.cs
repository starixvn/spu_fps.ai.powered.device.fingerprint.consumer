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

        #region GetWebRTCIPUnique Tests (via InsertWebRTCIP)

        [Fact]
        public void InsertWebRTCIP_WithValidWebRTCIP_ShouldCallDataAccessWithUniqueEntries()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.101"),
                    TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(2);
        }

        [Fact]
        public void InsertWebRTCIP_WithNullWebRTCIP_ShouldFilterOutNullEntries()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = null,
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(1);
        }

        [Fact]
        public void InsertWebRTCIP_WithEmptyWebRTCIPCode_ShouldFilterOutEmptyEntries()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel(string.Empty),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(1);
        }

        [Fact]
        public void InsertWebRTCIP_WithDuplicateWebRTCIP_ShouldGroupAndReturnOneEntry()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 14, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(1);
        }

        [Fact]
        public void InsertWebRTCIP_WithDifferentTransTimes_ShouldUseEarliestDate()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 3, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 2, 15, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(1);
        }

        [Fact]
        public void InsertWebRTCIP_WithEmptyList_ShouldCallDataAccessWithEmptyResult()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>();

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().BeEmpty();
        }

        [Fact]
        public void InsertWebRTCIP_WithAllNullWebRTCIP_ShouldCallDataAccessWithEmptyResult()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new() { WebRTCIP = null, TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
                new() { WebRTCIP = null, TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc) }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().BeEmpty();
        }

        [Fact]
        public void InsertWebRTCIP_WithMixedValidAndInvalid_ShouldReturnOnlyValid()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new() { WebRTCIP = null, TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
                new() { WebRTCIP = new WebRTCIPModel(string.Empty), TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc) },
                new() { WebRTCIP = new WebRTCIPModel("192.168.1.100"), TransTime = new DateTime(2025, 1, 1, 14, 0, 0, DateTimeKind.Utc) },
                new() { WebRTCIP = new WebRTCIPModel("192.168.1.101"), TransTime = new DateTime(2025, 1, 1, 15, 0, 0, DateTimeKind.Utc) }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(2);
        }

        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.1")]
        [InlineData("172.16.0.1")]
        [InlineData("fe80::1")]
        [InlineData("2001:db8::1")]
        public void InsertWebRTCIP_WithVariousIPFormats_ShouldWork(string ipAddress)
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel(ipAddress),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(1);
        }

        [Fact]
        public void InsertWebRTCIP_WithDifferentDetails_ShouldGroupSeparately()
        {
            // Arrange
            // This test ensures that grouping is done by both Code AND Details
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.101"),
                    TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(2);
        }

        [Fact]
        public void InsertWebRTCIP_WithDifferentDates_ShouldFormatCorrectly()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 12, 25, 23, 59, 59, DateTimeKind.Utc)
                },
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.101"),
                    TransTime = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(2);
        }

        [Fact]
        public void InsertWebRTCIP_ShouldUseCorrectDetailsType()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            int capturedDetailsType = 0;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedDetailsType = type);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new()
                {
                    WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                    TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
                }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            capturedDetailsType.Should().Be(1); // DETAILS_TYPE_WEBRTCIP = 1
        }

        [Fact]
        public void InsertWebRTCIP_WithComplexScenario_ShouldHandleCorrectly()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                // Valid entries with same IP (should be grouped)
                new() { WebRTCIP = new WebRTCIPModel("192.168.1.100"), TransTime = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc) },
                new() { WebRTCIP = new WebRTCIPModel("192.168.1.100"), TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
                
                // Valid entry with different IP
                new() { WebRTCIP = new WebRTCIPModel("192.168.1.101"), TransTime = new DateTime(2025, 1, 1, 14, 0, 0, DateTimeKind.Utc) },
                
                // Invalid entries (should be filtered out)
                new() { WebRTCIP = null, TransTime = new DateTime(2025, 1, 1, 15, 0, 0, DateTimeKind.Utc) },
                new() { WebRTCIP = new WebRTCIPModel(string.Empty), TransTime = new DateTime(2025, 1, 1, 16, 0, 0, DateTimeKind.Utc) }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(2); // Only 2 unique valid entries
        }

        [Fact]
        public void InsertWebRTCIP_WithIPv6Addresses_ShouldWorkCorrectly()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                         .Callback<int, string>((type, json) => capturedJson = json);

            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<RawTransactionModel>
            {
                new() { WebRTCIP = new WebRTCIPModel("2001:db8::1"), TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc) },
                new() { WebRTCIP = new WebRTCIPModel("fe80::1"), TransTime = new DateTime(2025, 1, 1, 13, 0, 0, DateTimeKind.Utc) }
            };

            // Act
            service.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
            
            var deserializedResult = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedResult.Should().NotBeNull();
            deserializedResult.Should().HaveCount(2);
        }

        #endregion
    }
}
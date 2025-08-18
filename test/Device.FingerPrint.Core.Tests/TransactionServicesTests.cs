using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Model;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
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
            service.InsertRawTransaction(transactions, isTransOfDBExtra);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>(), isTransOfDBExtra), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertFPTagging_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var taggings = "tag1,tag2";
            var isTransOfDBExtra = true;

            // Act
            service.InsertFPTagging(taggings, isTransOfDBExtra);

            // Assert
            mockDataAccess.Verify(x => x.InsertFPTagging(taggings, isTransOfDBExtra), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertFPUserAgent_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var userAgentJson = "{\"userAgent\":\"test\"}";
            var isTransOfDBExtra = false;

            // Act
            service.InsertFPUserAgent(userAgentJson, isTransOfDBExtra);

            // Assert
            mockDataAccess.Verify(x => x.InsertFPUserAgent(userAgentJson, isTransOfDBExtra), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertJSChallengeInfo_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var jsChallengeInfoJson = "{\"challenge\":\"test\"}";
            var isTransOfDBExtra = true;

            // Act
            service.InsertJSChallengeInfo(jsChallengeInfoJson, isTransOfDBExtra);

            // Assert
            mockDataAccess.Verify(x => x.InsertJSChallengeInfo(jsChallengeInfoJson, isTransOfDBExtra), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertFPFingerPrint_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var fingerPrintJson = "{\"fingerprint\":\"test\"}";
            var isTransOfDBExtra = false;

            // Act
            service.InsertFPFingerPrint(fingerPrintJson, isTransOfDBExtra);

            // Assert
            mockDataAccess.Verify(x => x.InsertFPFingerPrint(fingerPrintJson, isTransOfDBExtra), Times.Once);
        }

        [Fact]
        public void TransactionService_InsertTransaction_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var transactions = new List<TransactionEntity>
            {
                new TransactionEntity { LoginName = "test" }
            };

            // Act
            service.InsertTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertTransaction(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void TransactionService_DeviceIdentify_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var transJson = "{\"device\":\"test\"}";
            var groupType = 1;

            // Act
            service.DeviceIdentify(transJson, groupType);

            // Assert
            mockDataAccess.Verify(x => x.DeviceIdentify(transJson, groupType), Times.Once);
        }

        [Fact]
        public void TransactionService_DeviceIdentifyNoFingerprint_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var rawTransIds = "1,2,3";

            // Act
            service.DeviceIdentifyNoFingerprint(rawTransIds);

            // Assert
            mockDataAccess.Verify(x => x.DeviceIdentifyNoFingerprint(rawTransIds), Times.Once);
        }

        [Fact]
        public void TransactionService_GetDeviceIdentify_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var batchSize = 100;
            var numberOfBatch = 5;
            var expectedResult = (new List<DeviceIdentifyEntity>(), new List<DeviceIdentifyEntity>(), new List<ulong>());

            mockDataAccess.Setup(x => x.GetDeviceIdentify(batchSize, numberOfBatch))
                .Returns(expectedResult);

            // Act
            var result = service.GetDeviceIdentify(batchSize, numberOfBatch);

            // Assert
            result.Should().Be(expectedResult);
            mockDataAccess.Verify(x => x.GetDeviceIdentify(batchSize, numberOfBatch), Times.Once);
        }

        [Fact]
        public void TransactionService_MergeTransaction_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var transJson = "{\"merge\":\"test\"}";

            // Act
            service.MergeTransaction(transJson);

            // Assert
            mockDataAccess.Verify(x => x.UpdateMergeTransaction(transJson), Times.Once);
        }

        [Fact]
        public void TransactionService_GetMergeTransaction_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var batchSize = 50;
            var retry = 3;
            var expectedResult = new List<MergeTransEntity>();

            mockDataAccess.Setup(x => x.GetMergeTransaction(batchSize, retry))
                .Returns(expectedResult);

            // Act
            var result = service.GetMergeTransaction(batchSize, retry);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetMergeTransaction(batchSize, retry), Times.Once);
        }

        [Fact]
        public void TransactionService_GetForceMergeTransaction_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var batchSize = 50;
            var retry = 3;
            var expectedResult = new List<MergeTransEntity>();

            mockDataAccess.Setup(x => x.GetForceMergeTransaction(batchSize, retry))
                .Returns(expectedResult);

            // Act
            var result = service.GetForceMergeTransaction(batchSize, retry);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetForceMergeTransaction(batchSize, retry), Times.Once);
        }

        [Fact]
        public void TransactionService_ForceMergeTransaction_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var transJson = "{\"forceMerge\":\"test\"}";

            // Act
            service.ForceMergeTransaction(transJson);

            // Assert
            mockDataAccess.Verify(x => x.UpdateForceMergeTransaction(transJson), Times.Once);
        }

        [Fact]
        public void TransactionService_GetTransactionEvolution_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var batchSize = 100;
            var expectedResult = new List<TransactionEvolutionEntity>();

            mockDataAccess.Setup(x => x.GetTransactionEvolution(batchSize))
                .Returns(expectedResult);

            // Act
            var result = service.GetTransactionEvolution(batchSize);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetTransactionEvolution(batchSize), Times.Once);
        }

        [Fact]
        public void TransactionService_GetTransactionEvolutionAttributes_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var fingerPrintIds = "1,2,3";
            var browserFamilyCode = "chrome";
            var expectedResult = new List<TransactionEvolutionAttributesEntity>();

            mockDataAccess.Setup(x => x.GetTransactionEvolutionAttributes(fingerPrintIds, browserFamilyCode))
                .Returns(expectedResult);

            // Act
            var result = service.GetTransactionEvolutionAttributes(fingerPrintIds, browserFamilyCode);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetTransactionEvolutionAttributes(fingerPrintIds, browserFamilyCode), Times.Once);
        }

        [Fact]
        public void TransactionService_EvolutionNewDevice_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var transJson = "{\"evolution\":\"new\"}";

            // Act
            service.EvolutionNewDevice(transJson);

            // Assert
            mockDataAccess.Verify(x => x.EvolutionNewDevice(transJson), Times.Once);
        }

        [Fact]
        public void TransactionService_EvolutionRecoverDevice_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var transJson = "{\"evolution\":\"recover\"}";

            // Act
            service.EvolutionRecoverDevice(transJson);

            // Assert
            mockDataAccess.Verify(x => x.EvolutionRecoverDevice(transJson), Times.Once);
        }

        [Fact]
        public void TransactionService_GetTransactionHistoryByTransIdSubscriber_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var subscriberId = 123;
            var transId = 456L;
            var limit = 10;
            var expectedResult = new List<TransactionHistoryEntity>();

            mockDataAccess.Setup(x => x.GetTransactionHistoryByTransIdSubscriber(subscriberId, transId, limit))
                .Returns(expectedResult);

            // Act
            var result = service.GetTransactionHistoryByTransIdSubscriber(subscriberId, transId, limit);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetTransactionHistoryByTransIdSubscriber(subscriberId, transId, limit), Times.Once);
        }

        [Fact]
        public void TransactionService_GetAssociationByAccountSubscriber_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var subscriberId = 123;
            var account = "testuser";
            var expectedResult = (new List<AssociationDeviceEntity>(), new List<AssociationAccountEntity>());

            mockDataAccess.Setup(x => x.GetAssociationByAccountSubscriber(subscriberId, account))
                .Returns(expectedResult);

            // Act
            var result = service.GetAssociationByAccountSubscriber(subscriberId, account);

            // Assert
            result.Should().Be(expectedResult);
            mockDataAccess.Verify(x => x.GetAssociationByAccountSubscriber(subscriberId, account), Times.Once);
        }

        [Fact]
        public void TransactionService_GetAssociationBySubscriberDevice_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var subscriberId = 123;
            var deviceCode = "device123";
            var fromDate = DateTime.Now.AddDays(-30);
            var toDate = DateTime.Now;
            var expectedResult = new List<AssociationAccountUsageEntity>();

            mockDataAccess.Setup(x => x.GetAssociationBySubscriberDevice(subscriberId, deviceCode, fromDate, toDate))
                .Returns(expectedResult);

            // Act
            var result = service.GetAssociationBySubscriberDevice(subscriberId, deviceCode, fromDate, toDate);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetAssociationBySubscriberDevice(subscriberId, deviceCode, fromDate, toDate), Times.Once);
        }

        [Fact]
        public void TransactionService_GetTransactionInfo_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var lastTransID = 1000UL;
            var batchSize = 50;
            var expectedResult = new List<TransactionInfoEntity>();

            mockDataAccess.Setup(x => x.GetTransactionInfo(lastTransID, batchSize))
                .Returns(expectedResult);

            // Act
            var result = service.GetTransactionInfo(lastTransID, batchSize);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetTransactionInfo(lastTransID, batchSize), Times.Once);
        }

        [Fact]
        public void TransactionService_GetTransactionHistory_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var param = new TransactionParamModel();
            var expectedResult = new TransactionResultModel();

            mockDataAccess.Setup(x => x.GetTransactionBySubscriber(param))
                .Returns(expectedResult);

            // Act
            var result = service.GetTransactionHistory(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetTransactionBySubscriber(param), Times.Once);
        }

        [Fact]
        public void TransactionService_GetLastestTransactionBySubscriber_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var param = new LatestTransactionParamModel();
            var expectedResult = new LatestTransactionResultModel();

            mockDataAccess.Setup(x => x.GetLastestTransactionBySubscriber(param))
                .Returns(expectedResult);

            // Act
            var result = service.GetLastestTransactionBySubscriber(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetLastestTransactionBySubscriber(param), Times.Once);
        }

        [Fact]
        public void TransactionService_GetTransactionHistoryByDevice_CallsDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<ITransactionDataAccess>();
            var service = new TransactionService(mockDataAccess.Object);
            var param = new TransactionHistoryByDeviceParamModel();
            var expectedResult = new TransactionHistoryByDeviceResultModel();

            mockDataAccess.Setup(x => x.GetTransactionHistoryByDevice(param))
                .Returns(expectedResult);

            // Act
            var result = service.GetTransactionHistoryByDevice(param);

            // Assert
            result.Should().BeSameAs(expectedResult);
            mockDataAccess.Verify(x => x.GetTransactionHistoryByDevice(param), Times.Once);
        }
    }
}

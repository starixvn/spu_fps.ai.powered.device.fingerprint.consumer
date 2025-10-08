using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Model;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class TransactionServicesTests
    {
        private readonly Mock<ITransactionDataAccess> mockDataAccess;
        private readonly TransactionService transactionService;

        public TransactionServicesTests()
        {
            mockDataAccess = new Mock<ITransactionDataAccess>();
            transactionService = new TransactionService(mockDataAccess.Object);
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidDataAccess_ShouldCreateInstance()
        {
            // Arrange & Act
            var service = new TransactionService(mockDataAccess.Object);
            
            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullDataAccess_ShouldCreateInstanceButFailOnMethodCall()
        {
            // Arrange & Act
            var service = new TransactionService(null!);
            
            // Assert - Constructor should succeed, but method calls should fail
            service.Should().NotBeNull();
            
            // Method calls should throw NullReferenceException
            var transactions = new List<RawTransactionModel> { CreateSampleRawTransaction("test", "success") };
            Assert.Throws<NullReferenceException>(() => service.InsertRawTransaction(transactions));
        }

        #endregion

        #region InsertRawTransaction Tests

        [Fact]
        public void InsertRawTransaction_WithValidTransactions_CallsDataAccess()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                CreateSampleRawTransaction("test", "success")
            };

            // Act
            transactionService.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void InsertRawTransaction_WithEmptyList_CallsDataAccessWithEmptyArray()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>();

            // Act
            transactionService.InsertRawTransaction(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.Is<string>(json => 
                json == "[]")), Times.Once);
        }

        [Fact]
        public void InsertRawTransaction_WithMultipleTransactions_TransformsAllCorrectly()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                CreateSampleRawTransaction("login", "success"),
                CreateSampleRawTransaction("transfer", "failed"),
                CreateSampleRawTransaction("logout", "success")
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json);

            // Act
            transactionService.InsertRawTransaction(transactions);

            // Assert
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities.Should().HaveCount(3);
            deserializedEntities![0].Action.Should().Be("login");
            deserializedEntities[1].Action.Should().Be("transfer");
            deserializedEntities[2].Action.Should().Be("logout");
        }

        [Fact]
        public void InsertRawTransaction_WithComplexTransactionData_MapsAllFieldsCorrectly()
        {
            // Arrange
            var webRtcModel = new WebRTCIPModel("192.168.1.1,10.0.0.1");
            var transaction = new RawTransactionModel
            {
                LoginName = "testuser",
                SubscriberName = "TestSubscriber",
                TransTime = new DateTime(2023, 10, 15, 14, 30, 0),
                CreatedDate = new DateTime(2023, 10, 15, 14, 30, 0),
                DeviceCode = "DEV123",
                FingerprintCode = "FP456",
                FingerprintMoreInfo = "Additional info",
                UserAgent = "Mozilla/5.0",
                IP = "192.168.1.100",
                FakeIP = "10.0.0.100",
                IPId = new BigInteger(12345),
                IsIPV6 = false,
                PluginID = 1,
                URL = "https://example.com",
                Action = "login",
                ActionResult = "success",
                InvalidDevice = "false",
                TransStatus = 1,
                FlaggedCode = "FLAG001",
                BotDetection = "human",
                BotComponent = "none",
                ChallengeCode = "CHAL123",
                IsIncognitoMode = false,
                MBDeviceCodeMediaDRM = "MB_DRM_001",
                MBDeviceCodeMachine = "MB_MAC_001",
                MBDeviceCodeTagging = "MB_TAG_001",
                MBIsMobileApp = true,
                MBIsEmulator = false,
                MBModelName = "iPhone 12",
                MBManufacturer = "Apple",
                MBBrand = "Apple",
                MBSDKVersion = "15.0",
                MBOS = "iOS",
                MBTimezone = "UTC+7",
                MBLanguage = "en-US",
                MBCountry = "US",
                MBDeviceDetails = "Device details",
                WebRTCIP = webRtcModel,
                FPPatternCode01 = "PATTERN01",
                FPPatternCode02 = "PATTERN02",
                FPVersion = "1.0",
                TransFlow = 1
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json);

            // Act
            transactionService.InsertRawTransaction(new[] { transaction });

            // Assert
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            var entity = deserializedEntities!.First();
            
            entity.LoginName.Should().Be("testuser");
            entity.SubscriberName.Should().Be("TestSubscriber");
            entity.DeviceCode.Should().Be("DEV123");
            entity.FingerprintCode.Should().Be("FP456");
            entity.UserAgent.Should().Be("Mozilla/5.0");
            entity.IP.Should().Be("192.168.1.100");
            entity.Action.Should().Be("login");
            entity.ActionResult.Should().Be("success");
            entity.MBIsMobileApp.Should().BeTrue();
            entity.MBIsEmulator.Should().BeFalse();
            entity.WebRTCIPCode.Should().Be(webRtcModel.Code);
            entity.FPPatternCode01.Should().Be("PATTERN01");
            entity.FPPatternCode02.Should().Be("PATTERN02");
            entity.FingerprintVersion.Should().Be("1.0");
            entity.TransFlow.Should().Be(1);
        }

        [Fact]
        public void InsertRawTransaction_WithNullWebRTCIP_SetsWebRTCIPCodeToNull()
        {
            // Arrange
            var transaction = CreateSampleRawTransaction("test", "success");
            transaction.WebRTCIP = null;

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawTransaction(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json);

            // Act
            transactionService.InsertRawTransaction(new[] { transaction });

            // Assert
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawTransactionEntity>>(capturedJson);
            deserializedEntities!.First().WebRTCIPCode.Should().BeNull();
        }

        #endregion

        #region InsertRawFingerprint Tests

        [Fact]
        public void InsertRawFingerprint_WithValidTransactions_CallsDataAccess()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                CreateTransactionWithFingerprint()
            };

            // Act
            transactionService.InsertRawFingerprint(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawFingerprint(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void InsertRawFingerprint_WithEmptyList_CallsDataAccessWithEmptyArray()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>();

            // Act
            transactionService.InsertRawFingerprint(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawFingerprint(It.Is<string>(json => 
                json == "[]")), Times.Once);
        }

        [Fact]
        public void InsertRawFingerprint_WithDuplicateFingerprints_GroupsByFingerprintCodes()
        {
            // Arrange
            var fingerprint = CreateSampleFingerprint();
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel 
                { 
                    FingerPrint = fingerprint,
                    FPPatternCode01 = "PATTERN01",
                    FPPatternCode02 = "PATTERN02",
                    TransTime = new DateTime(2023, 10, 15)
                },
                new RawTransactionModel 
                { 
                    FingerPrint = fingerprint,
                    FPPatternCode01 = "PATTERN01",
                    FPPatternCode02 = "PATTERN02",
                    TransTime = new DateTime(2023, 10, 16)
                }
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawFingerprint(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json);

            // Act
            transactionService.InsertRawFingerprint(transactions);

            // Assert
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawFingerprintEntity>>(capturedJson);
            deserializedEntities.Should().HaveCount(1);
            deserializedEntities!.First().CreatedDate.Should().Be(new DateTime(2023, 10, 15));
        }

        [Fact]
        public void InsertRawFingerprint_WithValidData_MapsAllFieldsCorrectly()
        {
            // Arrange
            var fingerprint = new APFingerprintModel
            {
                Au = "audio-data",
                Br = "browser-data",
                Gp = "graphics-data",
                Hw = "hardware-data",
                Pf = "preferences-data",
                AudioCode = "AUDIO001",
                BrowserCode = "BROWSER001",
                GraphicCode = "GRAPHICS001",
                HardwareCode = "HARDWARE001",
                PreferencesCode = "PREFS001"
            };

            var transaction = new RawTransactionModel
            {
                FingerPrint = fingerprint,
                FPPatternCode01 = "PATTERN01",
                FPPatternCode02 = "PATTERN02",
                TransTime = new DateTime(2023, 10, 15, 14, 30, 0)
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertRawFingerprint(It.IsAny<string>()))
                          .Callback<string>(json => capturedJson = json);

            // Act
            transactionService.InsertRawFingerprint(new[] { transaction });

            // Assert
            var deserializedEntities = JsonConvert.DeserializeObject<List<InsertRawFingerprintEntity>>(capturedJson);
            var entity = deserializedEntities!.First();

            entity.AudioCode.Should().Be("AUDIO001");
            entity.BrowserCode.Should().Be("BROWSER001");
            entity.GraphicCode.Should().Be("GRAPHICS001");
            entity.HardwareCode.Should().Be("HARDWARE001");
            entity.PreferencesCode.Should().Be("PREFS001");
            entity.FPPatternCode01.Should().Be("PATTERN01");
            entity.FPPatternCode02.Should().Be("PATTERN02");
            entity.AudioDetails.Should().Be("audio-data");
            entity.BrowserDetails.Should().Be("browser-data");
            entity.GraphicDetails.Should().Be("graphics-data");
            entity.HardwareDetails.Should().Be("hardware-data");
            entity.PreferencesDetails.Should().Be("preferences-data");
            entity.CreatedDate.Should().Be(new DateTime(2023, 10, 15));
        }

        #endregion

        #region InsertWebRTCIP Tests

        [Fact]
        public void InsertWebRTCIP_WithValidTransactions_CallsDataAccessWithCorrectType()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                CreateTransactionWithWebRTC("192.168.1.1,10.0.0.1")
            };

            // Act
            transactionService.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void InsertWebRTCIP_WithEmptyList_CallsDataAccessWithEmptyArray()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>();

            // Act
            transactionService.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, "[]"), Times.Once);
        }

        [Fact]
        public void InsertWebRTCIP_WithNullWebRTCIP_FiltersOutTransaction()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel { WebRTCIP = null },
                CreateTransactionWithWebRTC("192.168.1.1")
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                          .Callback<int, string>((type, json) => capturedJson = json);

            // Act
            transactionService.InsertWebRTCIP(transactions);

            // Assert
            var deserializedData = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedData.Should().HaveCount(1);
        }

        [Fact]
        public void InsertWebRTCIP_WithEmptyWebRTCIPCode_FiltersOutTransaction()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel { WebRTCIP = new WebRTCIPModel("") },
                CreateTransactionWithWebRTC("192.168.1.1")
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                          .Callback<int, string>((type, json) => capturedJson = json);

            // Act
            transactionService.InsertWebRTCIP(transactions);

            // Assert
            var deserializedData = JsonConvert.DeserializeObject<List<object>>(capturedJson);
            deserializedData.Should().HaveCount(1);
        }

        [Fact]
        public void InsertWebRTCIP_WithDuplicateWebRTCIPs_GroupsByCodeAndDetails()
        {
            // Arrange
            var webRtcDetails = "192.168.1.1,10.0.0.1";
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel 
                { 
                    WebRTCIP = new WebRTCIPModel(webRtcDetails),
                    TransTime = new DateTime(2023, 10, 15, 14, 30, 0)
                },
                new RawTransactionModel 
                { 
                    WebRTCIP = new WebRTCIPModel(webRtcDetails),
                    TransTime = new DateTime(2023, 10, 16, 15, 30, 0)
                }
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                          .Callback<int, string>((type, json) => capturedJson = json);

            // Act
            transactionService.InsertWebRTCIP(transactions);

            // Assert
            var deserializedData = JsonConvert.DeserializeObject<List<dynamic>>(capturedJson);
            deserializedData.Should().HaveCount(1);
            
            string createdDate = deserializedData!.First().CreatedDate;
            createdDate.Should().Be("2023-10-15");
        }

        [Fact]
        public void InsertWebRTCIP_WithMultipleUniqueWebRTCIPs_CreatesMultipleEntries()
        {
            // Arrange
            var transactions = new List<RawTransactionModel>
            {
                CreateTransactionWithWebRTC("192.168.1.1"),
                CreateTransactionWithWebRTC("10.0.0.1"),
                CreateTransactionWithWebRTC("172.16.0.1")
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                          .Callback<int, string>((type, json) => capturedJson = json);

            // Act
            transactionService.InsertWebRTCIP(transactions);

            // Assert
            var deserializedData = JsonConvert.DeserializeObject<List<dynamic>>(capturedJson);
            deserializedData.Should().HaveCount(3);
        }

        [Fact]
        public void InsertWebRTCIP_WithValidData_FormatsDateCorrectly()
        {
            // Arrange
            var transaction = new RawTransactionModel
            {
                WebRTCIP = new WebRTCIPModel("192.168.1.1"),
                TransTime = new DateTime(2023, 10, 15, 14, 30, 45, 123)
            };

            string capturedJson = string.Empty;
            mockDataAccess.Setup(x => x.InsertWebRTCIP(It.IsAny<int>(), It.IsAny<string>()))
                          .Callback<int, string>((type, json) => capturedJson = json);

            // Act
            transactionService.InsertWebRTCIP(new[] { transaction });

            // Assert
            var deserializedData = JsonConvert.DeserializeObject<List<dynamic>>(capturedJson);
            string createdDate = deserializedData!.First().CreatedDate;
            createdDate.Should().Be("2023-10-15");
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void AllMethods_WithComplexDataSet_WorkTogether()
        {
            // Arrange
            var fingerprint = CreateSampleFingerprint();
            var webRtc = new WebRTCIPModel("192.168.1.1,10.0.0.1");
            
            var transactions = new List<RawTransactionModel>
            {
                new RawTransactionModel
                {
                    Action = "login",
                    ActionResult = "success",
                    FingerPrint = fingerprint,
                    FPPatternCode01 = "PATTERN01",
                    FPPatternCode02 = "PATTERN02",
                    WebRTCIP = webRtc,
                    TransTime = new DateTime(2023, 10, 15),
                    FPVersion = "1.0"
                }
            };

            // Act
            transactionService.InsertRawTransaction(transactions);
            transactionService.InsertRawFingerprint(transactions);
            transactionService.InsertWebRTCIP(transactions);

            // Assert
            mockDataAccess.Verify(x => x.InsertRawTransaction(It.IsAny<string>()), Times.Once);
            mockDataAccess.Verify(x => x.InsertRawFingerprint(It.IsAny<string>()), Times.Once);
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void AllMethods_WithNullInputs_HandleGracefully()
        {
            // Arrange
            List<RawTransactionModel> nullTransactions = null!;
            var emptyTransactions = new List<RawTransactionModel>();

            // Act & Assert - Null collections should throw NullReferenceException or ArgumentNullException
            var insertRawTransactionException = Assert.ThrowsAny<Exception>(() => transactionService.InsertRawTransaction(nullTransactions));
            var insertRawFingerprintException = Assert.ThrowsAny<Exception>(() => transactionService.InsertRawFingerprint(nullTransactions));
            var insertWebRTCIPException = Assert.ThrowsAny<Exception>(() => transactionService.InsertWebRTCIP(nullTransactions));

            // Verify we get some kind of null-related exception
            insertRawTransactionException.Should().BeOfType<ArgumentNullException>();
            insertRawFingerprintException.Should().BeOfType<ArgumentNullException>();
            insertWebRTCIPException.Should().BeOfType<ArgumentNullException>();

            // Empty collections should work
            transactionService.InsertRawTransaction(emptyTransactions);
            transactionService.InsertRawFingerprint(emptyTransactions);
            transactionService.InsertWebRTCIP(emptyTransactions);

            mockDataAccess.Verify(x => x.InsertRawTransaction("[]"), Times.Once);
            mockDataAccess.Verify(x => x.InsertRawFingerprint("[]"), Times.Once);
            mockDataAccess.Verify(x => x.InsertWebRTCIP(1, "[]"), Times.Once);
        }

        #endregion

        #region Helper Methods

        private static RawTransactionModel CreateSampleRawTransaction(string action, string actionResult)
        {
            return new RawTransactionModel
            {
                Action = action,
                ActionResult = actionResult,
                TransTime = DateTime.Now,
                CreatedDate = DateTime.Now,
                FPVersion = "1.0"
            };
        }

        private static RawTransactionModel CreateTransactionWithFingerprint()
        {
            return new RawTransactionModel
            {
                FingerPrint = CreateSampleFingerprint(),
                FPPatternCode01 = "PATTERN01",
                FPPatternCode02 = "PATTERN02",
                TransTime = DateTime.Now,
                FPVersion = "1.0"
            };
        }

        private static RawTransactionModel CreateTransactionWithWebRTC(string webRtcDetails)
        {
            return new RawTransactionModel
            {
                WebRTCIP = new WebRTCIPModel(webRtcDetails),
                TransTime = DateTime.Now,
                FPVersion = "1.0"
            };
        }

        private static APFingerprintModel CreateSampleFingerprint()
        {
            return new APFingerprintModel
            {
                Au = "sample-audio",
                Br = "sample-browser",
                Gp = "sample-graphics",
                Hw = "sample-hardware",
                Pf = "sample-preferences",
                AudioCode = "AUDIO001",
                BrowserCode = "BROWSER001",
                GraphicCode = "GRAPHICS001",
                HardwareCode = "HARDWARE001",
                PreferencesCode = "PREFS001"
            };
        }

        #endregion
    }
}

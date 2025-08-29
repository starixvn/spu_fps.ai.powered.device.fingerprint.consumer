using Xunit;
using Moq;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.Core;
using Fanex.Logging;
using System;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class MobileRawTransactionServiceTests
    {
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly MobileRawTransactionService _service;

        public MobileRawTransactionServiceTests()
        {
            // Setup logging engines for tests
            var mockLogger = new Mock<ILogger>();

            _mockAppSettings = new Mock<IAppSettings>();
            _mockAppSettings.Setup(x => x.Environment).Returns("Test");
            _service = new MobileRawTransactionService(_mockAppSettings.Object);
        }

        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            Assert.NotNull(_service);
        }

        [Fact]
        public void TransformRawTransaction_Should_Return_RawTransactionModel()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testuser",
                Subscriber = "testsub",
                SubscriberId = 123,
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = new MobileModel
                {
                    MediaDRMCode = "drm123",
                    MachineCode = "machine456",
                    CustomCode = "custom789",
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS 15",
                    Timezone = "UTC+7",
                    SystemLanguage = "en",
                    Country = "US",
                    SdkVersion = "1.0",
                    DeviceDetails = "iPhone details"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("testuser", result.LoginName);
            Assert.Equal("testsub", result.SubscriberName);
            Assert.Equal(123, result.SubscriberId);
            Assert.Equal("login", result.Action);
            Assert.Equal("success", result.ActionResult);
            Assert.Equal("drm123", result.MBDeviceCodeMediaDRM);
            Assert.Equal("machine456", result.MBDeviceCodeMachine);
            Assert.Equal("custom789", result.MBDeviceCodeTagging);
            Assert.True(result.MBIsMobileApp);
            Assert.False(result.MBIsEmulator);
            Assert.Equal("iPhone 12", result.MBModelName);
            Assert.Equal("Apple", result.MBManufacturer);
            Assert.Equal("Apple", result.MBBrand);
            Assert.Equal("iOS 15", result.MBOS);
            Assert.Equal("UTC+7", result.MBTimezone);
            Assert.Equal("en", result.MBLanguage);
            Assert.Equal("US", result.MBCountry);
            Assert.Equal("1.0", result.MBSDKVersion);
            Assert.Equal("iPhone details", result.MBDeviceDetails);
            Assert.Equal("192.168.1.1", result.IP);
            Assert.Equal("Human", result.FlaggedCode);
        }

        [Fact]
        public void TransformRawTransaction_WithEmptyDeviceCodes_Should_Return_EmptyDevice_Flag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testuser",
                Subscriber = "testsub",
                SubscriberId = 123,
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = new MobileModel
                {
                    MediaDRMCode = null,
                    MachineCode = "",
                    CustomCode = "",
                    IsMobileAppTrans = true
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("EmptyDevice", result.FlaggedCode);
        }

        [Fact]
        public void TransformRawTransaction_WithValidIP_Should_Transform_IP_Correctly()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testuser",
                Subscriber = "testsub",
                SubscriberId = 123,
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "  192.168.1.1  ", // IP with spaces
                Mobile = new MobileModel
                {
                    MachineCode = "machine123",
                    IsMobileAppTrans = true
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("192.168.1.1", result.IP); // Should be trimmed
        }
    }
}

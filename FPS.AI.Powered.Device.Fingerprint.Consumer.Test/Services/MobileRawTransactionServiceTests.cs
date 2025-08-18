using Device.FingerPrint.Core;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Services
{
    public class MobileRawTransactionServiceTests
    {
        private readonly Mock<IUserAgentService> _mockUserAgentService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly MobileRawTransactionService _service;

        public MobileRawTransactionServiceTests()
        {
            _mockUserAgentService = new Mock<IUserAgentService>();
            _mockAppSettings = new Mock<IAppSettings>();

            _mockAppSettings.Setup(x => x.Environment).Returns("TEST");

            _service = new MobileRawTransactionService(_mockUserAgentService.Object, _mockAppSettings.Object);
        }

        [Fact]
        public void TransformRawTransaction_WithValidMobileData_ShouldReturnValidTransaction()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                SubscriberId = "test-subscriber-id",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = new MobileModel
                {
                    MediaDRMCode = "test-media-drm-code",
                    MachineCode = "test-machine-code",
                    CustomCode = "test-custom-code",
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS",
                    Timezone = "UTC",
                    SystemLanguage = "en-US",
                    Country = "US",
                    SdkVersion = "14.6",
                    DeviceDetails = "test-device-details"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.LoginName.Should().Be("test-account");
            result.SubscriberName.Should().Be("test-subscriber");
            result.SubscriberId.Should().Be("test-subscriber-id");
            result.Action.Should().Be("login");
            result.ActionResult.Should().Be("success");
            result.MBDeviceCodeMediaDRM.Should().Be("test-media-drm-code");
            result.MBDeviceCodeMachine.Should().Be("test-machine-code");
            result.MBDeviceCodeTagging.Should().Be("test-custom-code");
            result.MBIsMobileApp.Should().BeTrue();
            result.MBIsEmulator.Should().BeFalse();
            result.MBModelName.Should().Be("iPhone 12");
            result.MBManufacturer.Should().Be("Apple");
            result.MBBrand.Should().Be("Apple");
            result.MBOS.Should().Be("iOS");
            result.MBTimezone.Should().Be("UTC");
            result.MBLanguage.Should().Be("en-US");
            result.MBCountry.Should().Be("US");
            result.MBSDKVersion.Should().Be("14.6");
            result.MBDeviceDetails.Should().Be("test-device-details");
            result.FlaggedCode.Should().Be("0"); // Human
            result.IP.Should().Be("192.168.1.1");
            result.IsIPV6.Should().BeFalse();
        }

        [Fact]
        public void TransformRawTransaction_WithEmptyDeviceCodes_ShouldReturnEmptyDeviceFlag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = new MobileModel
                {
                    MediaDRMCode = "",
                    MachineCode = "",
                    CustomCode = "",
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.FlaggedCode.Should().Be("2"); // EmptyDevice
        }

        [Fact]
        public void TransformRawTransaction_WithNullDeviceCodes_ShouldReturnEmptyDeviceFlag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = new MobileModel
                {
                    MediaDRMCode = null,
                    MachineCode = null,
                    CustomCode = null,
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.FlaggedCode.Should().Be("2"); // EmptyDevice
        }

        [Fact]
        public void TransformRawTransaction_WithIPv6Address_ShouldSetIPv6Flag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "2001:0db8:85a3:0000:0000:8a2e:0370:7334",
                Mobile = new MobileModel
                {
                    MediaDRMCode = "test-media-drm-code",
                    MachineCode = "test-machine-code",
                    CustomCode = "test-custom-code",
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.IP.Should().Be("2001:0db8:85a3:0000:0000:8a2e:0370:7334");
            result.IsIPV6.Should().BeTrue();
        }

        [Fact]
        public void TransformRawTransaction_WithNullIP_ShouldHandleGracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = null,
                Mobile = new MobileModel
                {
                    MediaDRMCode = "test-media-drm-code",
                    MachineCode = "test-machine-code",
                    CustomCode = "test-custom-code",
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.IP.Should().BeNull();
        }

        [Fact]
        public void TransformRawTransaction_WithEmptyIP_ShouldHandleGracefully()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "",
                Mobile = new MobileModel
                {
                    MediaDRMCode = "test-media-drm-code",
                    MachineCode = "test-machine-code",
                    CustomCode = "test-custom-code",
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.IP.Should().Be("");
        }

        [Fact]
        public void TransformRawTransaction_WithException_ShouldReturnNull()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = null // This will cause an exception
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void TransformRawTransaction_WithEmulator_ShouldSetEmulatorFlag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = new MobileModel
                {
                    MediaDRMCode = "test-media-drm-code",
                    MachineCode = "test-machine-code",
                    CustomCode = "test-custom-code",
                    IsMobileAppTrans = true,
                    IsEmulator = true,
                    ModelName = "Android Emulator",
                    Manufacturer = "Google",
                    Brand = "Google",
                    OS = "Android"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.MBIsEmulator.Should().BeTrue();
        }

        [Fact]
        public void TransformRawTransaction_WithPartialDeviceCodes_ShouldReturnHumanFlag()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "test-account",
                Subscriber = "test-subscriber",
                TransDate = DateTime.Now,
                Action = "login",
                ActionResult = "success",
                IP = "192.168.1.1",
                Mobile = new MobileModel
                {
                    MediaDRMCode = "test-media-drm-code",
                    MachineCode = "",
                    CustomCode = "",
                    IsMobileAppTrans = true,
                    IsEmulator = false,
                    ModelName = "iPhone 12",
                    Manufacturer = "Apple",
                    Brand = "Apple",
                    OS = "iOS"
                }
            };

            // Act
            var result = _service.TransformRawTransaction(message);

            // Assert
            result.Should().NotBeNull();
            result.FlaggedCode.Should().Be("0"); // Human (at least one device code is present)
        }
    }
}

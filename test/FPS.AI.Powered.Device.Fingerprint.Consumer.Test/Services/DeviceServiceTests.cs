using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Services
{
    public class DeviceServiceTests
    {
        [Fact]
        public void GetDeviceInfo_WithValidDeviceCode_ShouldReturnDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "eyJkZXZpY2VDb2RlIjoidGVzdC1kZXZpY2UtY29kZSIsImRldmljZUlkIjoidmFsaWQtZGV2aWNlLWlkIn0="; // Base64 encoded JSON
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceFPCode.Should().Be("test-device-code");
            result.DeviceNo.Should().Be("valid-device-id");
        }

        [Fact]
        public void GetDeviceInfo_WithEmptyDeviceCode_ShouldReturnDefaultDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "";
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithNullDeviceCode_ShouldReturnDefaultDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            string deviceCode = null;
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidBase64_ShouldReturnDefaultDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "invalid-base64!@#";
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }
    }
}

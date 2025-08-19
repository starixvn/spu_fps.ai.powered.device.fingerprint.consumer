using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Tests.Services
{
    public class DeviceServiceTests
    {
        private readonly DeviceService _deviceService;

        public DeviceServiceTests()
        {
            _deviceService = new DeviceService();
        }

        [Fact]
        public void GetDeviceInfo_WhenDeviceCodeIsNull_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            string? deviceCode = null;
            var message = new MessageQueue();

            // Act
            var result = _deviceService.GetDeviceInfo(deviceCode!, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
            result.AppVersion.Should().BeEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WhenDeviceCodeIsEmpty_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            string deviceCode = "";
            var message = new MessageQueue();

            // Act
            var result = _deviceService.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
            result.AppVersion.Should().BeEmpty();
        }





        [Fact]
        public void GetDeviceInfo_WhenInvalidBase64_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            string deviceCode = "invalid-base64";
            var message = new MessageQueue();

            // Act
            var result = _deviceService.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
            result.AppVersion.Should().BeEmpty();
        }




    }
}

using Xunit;
using FluentAssertions;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Test
{
    public class DeviceServiceTests
    {
        [Fact]
        public void GetDeviceInfo_WithValidDeviceCode_ShouldReturnDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "eyJkZXZpY2VJZCI6InRlc3QxMjMiLCJhdHRyaWJ1dGVzIjp7ImJyb3dzZXIiOiJDaHJvbWUiLCJvcCI6IldpbmRvd3MifX0="; // Base64 encoded JSON
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WithEmptyDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "";
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WithNullDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            string deviceCode = null;
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WithNullStringDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "null";
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidBase64_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "invalid-base64-string";
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidJson_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var service = new DeviceService();
            var deviceCode = "aW52YWxpZC1qc29u"; // Base64 of "invalid-json"
            var message = new MessageQueue();

            // Act
            var result = service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().BeEmpty();
        }
    }
}

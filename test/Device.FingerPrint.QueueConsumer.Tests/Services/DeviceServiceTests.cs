using Device.FingerPrint.Core.Encoders;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.TransactionAPI.Model;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class DeviceServiceTests
    {
        private readonly DeviceService _service;
        private readonly MessageQueue _mockMessage;

        public DeviceServiceTests()
        {
            _service = new DeviceService();
            _mockMessage = new MessageQueue
            {
                Account = "testAccount",
                Action = "testAction"
            };
        }

        [Fact]
        public void GetDeviceInfo_WithNullDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Act
            var result = _service.GetDeviceInfo(null, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithEmptyDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Act
            var result = _service.GetDeviceInfo(string.Empty, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithWhitespaceDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Act
            var result = _service.GetDeviceInfo("   ", _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithNullStringDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Act
            var result = _service.GetDeviceInfo("null", _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithNullStringIgnoreCase_ShouldReturnEmptyDeviceInfo()
        {
            // Act
            var result = _service.GetDeviceInfo("NULL", _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidBase64_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var invalidBase64 = "invalid-base64-string!@#";

            // Act
            var result = _service.GetDeviceInfo(invalidBase64, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithValidBase64ButInvalidJson_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var invalidJson = "{ invalid json structure }";
            var base64EncodedInvalidJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(invalidJson));

            // Act
            var result = _service.GetDeviceInfo(base64EncodedInvalidJson, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("null")]
        [InlineData("NULL")]
        [InlineData("Null")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void GetDeviceInfo_WithVariousInvalidInputs_ShouldReturnEmptyDeviceInfo(string invalidInput)
        {
            // Act
            var result = _service.GetDeviceInfo(invalidInput, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WhenBase64DecodeThrowsException_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange - Create a string that will cause Base64 decode to fail
            var malformedBase64 = "SGVsbG8gV29ybGQ"; // Missing padding that might cause issues

            // Act
            var result = _service.GetDeviceInfo(malformedBase64, _mockMessage);

            // Assert - Should handle the exception gracefully and return empty device info
            Assert.NotNull(result);
            Assert.Equal(string.Empty, result.Tagging);
            Assert.Equal(string.Empty, result.F4);
            Assert.Equal(string.Empty, result.F5);
            Assert.Equal(string.Empty, result.Rtc);
            Assert.Equal(string.Empty, result.Version);
        }
    }
}

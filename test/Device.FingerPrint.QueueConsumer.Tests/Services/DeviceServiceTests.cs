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
        public void GetDeviceInfo_WithValidBase64EncodedDeviceInfo_ShouldReturnDeserializedDeviceInfo()
        {
            // Arrange
            var expectedDeviceInfo = new DeviceInfoModel
            {
                Tagging = "test-tag",
                F4 = "test-f4",
                F5 = "test-f5",
                Rtc = "test-rtc",
                Version = "1.0.0"
            };

            var json = JsonConvert.SerializeObject(expectedDeviceInfo, Device.FingerPrint.Core.JsonSerializerSettingsFactory.Instance);
            var base64EncodedDeviceInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            // Act
            var result = _service.GetDeviceInfo(base64EncodedDeviceInfo, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedDeviceInfo.Tagging, result.Tagging);
            Assert.Equal(expectedDeviceInfo.F4, result.F4);
            Assert.Equal(expectedDeviceInfo.F5, result.F5);
            Assert.Equal(expectedDeviceInfo.Rtc, result.Rtc);
            Assert.Equal(expectedDeviceInfo.Version, result.Version);
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

        [Fact]
        public void GetDeviceInfo_WithPartialDeviceInfo_ShouldReturnDeviceInfoWithAvailableProperties()
        {
            // Arrange - Using Newtonsoft.Json camelCase naming
            var partialDeviceInfo = new
            {
                tagging = "partial-tag", // Property name should match camelCase for Newtonsoft.Json
                f4 = "partial-f4"
                // Missing F5, Rtc, Version
            };

            var json = JsonConvert.SerializeObject(partialDeviceInfo);
            var base64EncodedDeviceInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            // Act
            var result = _service.GetDeviceInfo(base64EncodedDeviceInfo, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("partial-tag", result.Tagging);
            Assert.Equal("partial-f4", result.F4);
            Assert.Null(result.F5); // Should be null as it wasn't in the JSON
            Assert.Null(result.Rtc);
            Assert.Null(result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithEmptyJsonObject_ShouldReturnDeviceInfoWithNullProperties()
        {
            // Arrange
            var emptyJson = "{}";
            var base64EncodedEmptyJson = Convert.ToBase64String(Encoding.UTF8.GetBytes(emptyJson));

            // Act
            var result = _service.GetDeviceInfo(base64EncodedEmptyJson, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.Tagging);
            Assert.Null(result.F4);
            Assert.Null(result.F5);
            Assert.Null(result.Rtc);
            Assert.Null(result.Version);
        }

        [Fact]
        public void GetDeviceInfo_WithCamelCaseProperties_ShouldDeserializeCorrectly()
        {
            // Arrange - Using correct property names for Newtonsoft.Json with CamelCasePropertyNamesContractResolver
            var deviceInfoWithCamelCase = new
            {
                tagging = "camel-tag", // Property name should be 'tagging' for Newtonsoft.Json camelCase
                f4 = "camel-f4",
                f5 = "camel-f5",
                rtc = "camel-rtc",
                version = "1.5.0" // Property name should be 'version' for Newtonsoft.Json camelCase
            };

            var json = JsonConvert.SerializeObject(deviceInfoWithCamelCase);
            var base64EncodedDeviceInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            // Act
            var result = _service.GetDeviceInfo(base64EncodedDeviceInfo, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("camel-tag", result.Tagging);
            Assert.Equal("camel-f4", result.F4);
            Assert.Equal("camel-f5", result.F5);
            Assert.Equal("camel-rtc", result.Rtc);
            Assert.Equal("1.5.0", result.Version);
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
        public void GetDeviceInfo_WithComplexDeviceInfo_ShouldDeserializeAllProperties()
        {
            // Arrange
            var complexDeviceInfo = new DeviceInfoModel
            {
                Tagging = "complex-device-tag-123",
                F4 = "fingerprint-feature-4-data",
                F5 = "fingerprint-feature-5-data",
                Rtc = "real-time-clock-data-456",
                Version = "2.1.3-beta"
            };

            var json = JsonConvert.SerializeObject(complexDeviceInfo, Device.FingerPrint.Core.JsonSerializerSettingsFactory.Instance);
            var base64EncodedDeviceInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            // Act
            var result = _service.GetDeviceInfo(base64EncodedDeviceInfo, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(complexDeviceInfo.Tagging, result.Tagging);
            Assert.Equal(complexDeviceInfo.F4, result.F4);
            Assert.Equal(complexDeviceInfo.F5, result.F5);
            Assert.Equal(complexDeviceInfo.Rtc, result.Rtc);
            Assert.Equal(complexDeviceInfo.Version, result.Version);
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

        [Fact]
        public void GetDeviceInfo_WithSpecialCharactersInDeviceInfo_ShouldHandleCorrectly()
        {
            // Arrange
            var deviceInfoWithSpecialChars = new DeviceInfoModel
            {
                Tagging = "tag-with-特殊字符-and-émojis-🚀",
                F4 = "f4-with-symbols-@#$%^&*()",
                F5 = "f5-with-quotes-\"single'quotes\"",
                Rtc = "rtc-with-newlines\n\r\t",
                Version = "v1.0.0-alpha+build.123"
            };

            var json = JsonConvert.SerializeObject(deviceInfoWithSpecialChars, Device.FingerPrint.Core.JsonSerializerSettingsFactory.Instance);
            var base64EncodedDeviceInfo = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            // Act
            var result = _service.GetDeviceInfo(base64EncodedDeviceInfo, _mockMessage);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(deviceInfoWithSpecialChars.Tagging, result.Tagging);
            Assert.Equal(deviceInfoWithSpecialChars.F4, result.F4);
            Assert.Equal(deviceInfoWithSpecialChars.F5, result.F5);
            Assert.Equal(deviceInfoWithSpecialChars.Rtc, result.Rtc);
            Assert.Equal(deviceInfoWithSpecialChars.Version, result.Version);
        }
    }
}

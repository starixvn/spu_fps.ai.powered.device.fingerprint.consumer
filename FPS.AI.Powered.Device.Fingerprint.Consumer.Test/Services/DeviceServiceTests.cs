using Device.FingerPrint.Core.Encoders;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Services
{
    public class DeviceServiceTests
    {
        private readonly DeviceService _service;

        public DeviceServiceTests()
        {
            _service = new DeviceService();
        }

        [Fact]
        public void GetDeviceInfo_WithValidDeviceCode_ShouldReturnDeviceInfo()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device-code",
                AppVersion = "1.0.0",
                TimeZone = "UTC",
                UserAgent = "Mozilla/5.0",
                DeviceId = "valid-device-id",
                Screen = new DeviceScreen
                {
                    Width = 1920,
                    Height = 1080
                },
                HRef = "https://example.com",
                BotD = new BotD
                {
                    Result = new BotDResult
                    {
                        Bot = false,
                        BotKind = ""
                    }
                }
            };

            var deviceCode = Base64.Encode(JsonConvert.SerializeObject(deviceInfo));
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceFPCode.Should().Be("test-device-code");
            result.AppVersion.Should().Be("1.0.0");
            result.DeviceNo.Should().Be("valid-device-id");
            result.InvalidDeviceId.Should().BeFalse();
        }

        [Fact]
        public void GetDeviceInfo_WithEmptyDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var deviceCode = "";
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().Be("N/A");
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithNullDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            string deviceCode = null;
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().Be("N/A");
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithNullStringDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var deviceCode = "null";
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().Be("N/A");
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidBase64_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var deviceCode = "invalid-base64-data";
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().Be("N/A");
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidJson_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var invalidJson = "invalid-json-data";
            var deviceCode = Base64.Encode(invalidJson);
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().Be("N/A");
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithBotDData_ShouldTransformBotDCorrectly()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device-code",
                DeviceId = "valid-device-id",
                BotD = new BotD
                {
                    Result = new BotDResult
                    {
                        Bot = true,
                        BotKind = "automated"
                    },
                    Detection = "{\"test\":{\"bot\":true}}",
                    Component = "test-component"
                }
            };

            var deviceCode = Base64.Encode(JsonConvert.SerializeObject(deviceInfo));
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceNo.Should().Be("valid-device-id");
            result.BotD.Should().NotBeNull();
            result.BotD.Result.Bot.Should().BeTrue();
            result.BotD.Result.BotKind.Should().Be("automated");
            result.BotD.Detection.Should().Be("{\"test\":{\"bot\":true}}");
            result.BotD.Component.Should().Be("test-component");
        }

        [Fact]
        public void GetDeviceInfo_WithScreenData_ShouldTransformScreenCorrectly()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device-code",
                DeviceId = "valid-device-id",
                Screen = new DeviceScreen
                {
                    Width = 1920,
                    Height = 1080,
                    ColorDepth = 24,
                    PixelDepth = 24,
                    AvailWidth = 1920,
                    AvailHeight = 1040
                }
            };

            var deviceCode = Base64.Encode(JsonConvert.SerializeObject(deviceInfo));
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.Screen.Should().NotBeNull();
            result.Screen.Width.Should().Be(1920);
            result.Screen.Height.Should().Be(1080);
            result.Screen.ColorDepth.Should().Be(24);
            result.Screen.PixelDepth.Should().Be(24);
            result.Screen.AvailWidth.Should().Be(1920);
            result.Screen.AvailHeight.Should().Be(1040);
        }

        [Fact]
        public void GetDeviceInfo_WithMissingCapturedDate_ShouldUseCurrentDate()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device-code",
                DeviceId = "valid-device-id",
                CapturedDate = null
            };

            var deviceCode = Base64.Encode(JsonConvert.SerializeObject(deviceInfo));
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.CapturedDate.Should().BeGreaterThan(0);
        }

        [Fact]
        public void GetDeviceInfo_WithCapturedDate_ShouldUseProvidedDate()
        {
            // Arrange
            var capturedDate = DateTime.Now.AddDays(-1);
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device-code",
                DeviceId = "valid-device-id",
                CapturedDate = capturedDate
            };

            var deviceCode = Base64.Encode(JsonConvert.SerializeObject(deviceInfo));
            var message = new MessageQueue();

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.CapturedDate.Should().Be((long)capturedDate.Ticks);
        }
    }
}

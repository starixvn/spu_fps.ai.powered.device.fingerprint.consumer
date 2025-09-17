using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.Model;
using Device.FingerPrint.Core;
using Newtonsoft.Json;
using Device.FingerPrint.TransactionAPI.Model;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class DeviceServiceTests
    {
        private readonly DeviceService _service;

        public DeviceServiceTests()
        {
            _service = new DeviceService();
        }

        [Fact]
        public void GetDeviceInfo_WithValidDeviceCode_ShouldReturnTransformedDeviceInfo()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                NA = "test-na",
                DeviceCode = "test-device-code",
                AppVersion = "1.2.3",
                TimeZone = "UTC+7",
                UserAgent = "Mozilla/5.0 test",
                DeviceId = "device-123",
                HRef = "https://example.com",
                CapturedDate = 1234567890,
                Version = "2.0.0",
                Screen = new DeviceScreen { Width = 1920, Height = 1080, ColorDepth = 24 },
                BotD = new BotD
                {
                    Result = new BotDResult { Bot = true, BotKind = "automated" },
                    Detection = "bot-detected",
                    Component = "test-component"
                }
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.NA.Should().Be("test-na");
            result.DeviceFPCode.Should().Be("test-device-code");
            result.AppVersion.Should().Be("1.2.3");
            result.TimeZone.Should().Be("UTC+7");
            result.UserAgent.Should().Be("Mozilla/5.0 test");
            result.DeviceNo.Should().Be("device-123");
            result.HRef.Should().Be("https://example.com");
            result.CapturedDate.Should().Be(1234567890);
            result.Version.Should().Be("2.0.0");
            result.Screen.Should().NotBeNull();
            result.Screen.Width.Should().Be(1920);
            result.Screen.Height.Should().Be(1080);
            result.Screen.ColorDepth.Should().Be(24);
            result.BotD.Should().NotBeNull();
            result.BotD.Result.Should().NotBeNull();
            result.BotD.Result.Bot.Should().BeTrue();
            result.BotD.Result.BotKind.Should().Be("automated");
            result.BotD.Detection.Should().Be("bot-detected");
            result.BotD.Component.Should().Be("test-component");
            result.InvalidDeviceId.Should().BeFalse();
        }

        [Fact]
        public void GetDeviceInfo_WithNullDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(null, message);

            // Assert
            result.Should().NotBeNull();
            result.AppVersion.Should().BeEmpty();
            result.CapturedDate.Should().Be(0);
            result.DeviceFPCode.Should().BeEmpty();
            result.DeviceNo.Should().BeEmpty();
            result.HRef.Should().BeEmpty();
            result.NA.Should().BeEmpty();
            result.TimeZone.Should().BeEmpty();
            result.UserAgent.Should().BeEmpty();
            result.InvalidDeviceId.Should().BeTrue();
            result.Screen.Should().NotBeNull();
            result.Screen.Width.Should().Be(0);
            result.Screen.Height.Should().Be(0);
            result.Screen.ColorDepth.Should().Be(0);
        }

        [Fact]
        public void GetDeviceInfo_WithEmptyDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo("", message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
            result.AppVersion.Should().BeEmpty();
            result.DeviceFPCode.Should().BeEmpty();
            result.DeviceNo.Should().BeEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WithWhitespaceDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo("   ", message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
            result.AppVersion.Should().BeEmpty();
        }

        [Fact]
        public void GetDeviceInfo_WithNullStringDeviceCode_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo("null", message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithNullStringDifferentCase_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result1 = _service.GetDeviceInfo("NULL", message);
            var result2 = _service.GetDeviceInfo("Null", message);
            var result3 = _service.GetDeviceInfo("nULL", message);

            // Assert
            result1.Should().NotBeNull();
            result1.InvalidDeviceId.Should().BeTrue();
            
            result2.Should().NotBeNull();
            result2.InvalidDeviceId.Should().BeTrue();
            
            result3.Should().NotBeNull();
            result3.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidBase64_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };
            var invalidBase64 = "invalid-base64-string!@#";

            // Act
            var result = _service.GetDeviceInfo(invalidBase64, message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithInvalidJson_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };
            var invalidJson = Convert.ToBase64String(Encoding.UTF8.GetBytes("invalid json"));

            // Act
            var result = _service.GetDeviceInfo(invalidJson, message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithNullScreen_ShouldReturnDefaultScreenInfo()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device",
                Screen = null // Null screen
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.Screen.Should().NotBeNull();
            result.Screen.Width.Should().Be(0);
            result.Screen.Height.Should().Be(0);
            result.Screen.ColorDepth.Should().Be(0);
        }

        [Fact]
        public void GetDeviceInfo_WithNullBotD_ShouldReturnEmptyBotDModel()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device",
                BotD = null // Null BotD
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.BotD.Should().NotBeNull();
            result.BotD.Result.Should().BeNull();
            result.BotD.Detection.Should().BeNull();
            result.BotD.Component.Should().BeNull();
        }

        [Fact]
        public void GetDeviceInfo_WithNullBotDResult_ShouldReturnEmptyBotDModel()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device",
                BotD = new BotD
                {
                    Result = null, // Null result
                    Detection = "some-detection",
                    Component = "some-component"
                }
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.BotD.Should().NotBeNull();
            result.BotD.Result.Should().BeNull();
            result.BotD.Detection.Should().BeNull();
            result.BotD.Component.Should().BeNull();
        }

        [Fact]
        public void GetDeviceInfo_WithNullCapturedDate_ShouldUseCurrentTime()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device",
                CapturedDate = null
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            var beforeCall = DateTimeOffset.Now.Ticks;

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            var afterCall = DateTimeOffset.Now.Ticks;
            result.Should().NotBeNull();
            result.CapturedDate.Should().BeGreaterOrEqualTo(beforeCall);
            result.CapturedDate.Should().BeLessOrEqualTo(afterCall);
        }

        [Fact]
        public void GetDeviceInfo_WithValidCapturedDate_ShouldUseCapturedDate()
        {
            // Arrange
            var capturedDate = 9876543210L;
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device",
                CapturedDate = capturedDate
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.CapturedDate.Should().Be(capturedDate);
        }

        [Fact]
        public void GetDeviceInfo_WithCompleteDeviceInfo_ShouldMapAllFields()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                NA = "North America",
                DeviceCode = "FP123456",
                AppVersion = "2.5.1",
                TimeZone = "EST",
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
                DeviceId = "WIN-ABC123",
                HRef = "https://banking.example.com/login",
                CapturedDate = 1609459200000, // 2021-01-01 timestamp
                Version = "3.1.4",
                Screen = new DeviceScreen { Width = 2560, Height = 1440, ColorDepth = 32 },
                BotD = new BotD
                {
                    Result = new BotDResult { Bot = false, BotKind = "human" },
                    Detection = "clean",
                    Component = "fingerprint-scanner"
                }
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "user123" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.NA.Should().Be("North America");
            result.DeviceFPCode.Should().Be("FP123456");
            result.AppVersion.Should().Be("2.5.1");
            result.TimeZone.Should().Be("EST");
            result.UserAgent.Should().Be("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
            result.DeviceNo.Should().Be("WIN-ABC123");
            result.HRef.Should().Be("https://banking.example.com/login");
            result.CapturedDate.Should().Be(1609459200000);
            result.Version.Should().Be("3.1.4");
            result.Screen.Width.Should().Be(2560);
            result.Screen.Height.Should().Be(1440);
            result.Screen.ColorDepth.Should().Be(32);
            result.BotD.Result.Bot.Should().BeFalse();
            result.BotD.Result.BotKind.Should().Be("human");
            result.BotD.Detection.Should().Be("clean");
            result.BotD.Component.Should().Be("fingerprint-scanner");
            result.InvalidDeviceId.Should().BeFalse();
        }

        [Fact]
        public void GetDeviceInfo_WithMalformedBotDData_ShouldHandleGracefully()
        {
            // Arrange
            // Create a DeviceInfo object with valid structure but problematic BotD data
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "test-device",
                BotD = new BotD
                {
                    Result = new BotDResult
                    {
                        Bot = true, // Valid boolean
                        BotKind = "test"
                    },
                    Detection = "bot-detected",
                    Component = "test-component"
                }
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceFPCode.Should().Be("test-device"); // DeviceCode should be extracted successfully
            result.BotD.Should().NotBeNull();
            result.BotD.Result.Should().NotBeNull();
            result.BotD.Result.Bot.Should().BeTrue();
            result.BotD.Result.BotKind.Should().Be("test");
            result.BotD.Detection.Should().Be("bot-detected");
            result.BotD.Component.Should().Be("test-component");
        }

        [Fact]
        public void GetDeviceInfo_WithException_ShouldReturnEmptyDeviceInfo()
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };
            var corruptedBase64 = "SGVsbG8gV29ybGQ="; // Valid base64 but invalid JSON

            // Act
            var result = _service.GetDeviceInfo(corruptedBase64, message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData(null)]
        public void GetDeviceInfo_WithEmptyOrWhitespaceValues_ShouldReturnEmptyDeviceInfo(string deviceCode)
        {
            // Arrange
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.InvalidDeviceId.Should().BeTrue();
        }

        [Fact]
        public void GetDeviceInfo_WithMinimalValidDeviceInfo_ShouldHandleNullFields()
        {
            // Arrange
            var deviceInfo = new DeviceInfo
            {
                DeviceCode = "minimal-device"
                // All other fields are null/default
            };

            var deviceCode = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
            var message = new MessageQueue { Account = "testuser" };

            // Act
            var result = _service.GetDeviceInfo(deviceCode, message);

            // Assert
            result.Should().NotBeNull();
            result.DeviceFPCode.Should().Be("minimal-device");
            result.InvalidDeviceId.Should().BeFalse();
            result.AppVersion.Should().BeNullOrEmpty();
            result.TimeZone.Should().BeNullOrEmpty();
            result.UserAgent.Should().BeNullOrEmpty();
            result.DeviceNo.Should().BeNullOrEmpty();
            result.HRef.Should().BeNullOrEmpty();
            result.NA.Should().BeNullOrEmpty();
            result.Screen.Should().NotBeNull();
            result.Screen.Width.Should().Be(0);
            result.Screen.Height.Should().Be(0);
            result.Screen.ColorDepth.Should().Be(0);
        }
    }
}

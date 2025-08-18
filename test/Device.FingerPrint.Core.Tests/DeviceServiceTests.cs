using Device.FingerPrint.Core.Encoders;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;
using Xunit;

namespace Device.FingerPrint.Core.Tests
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
				DeviceCode = "test-device",
				AppVersion = "1.0.0",
				TimeZone = "UTC",
				UserAgent = "test-ua",
				DeviceId = "test-id",
				HRef = "https://test.com",
				CapturedDate = 123456789,
				Version = "2.0.0",
				Screen = new DeviceScreen { Width = 1920, Height = 1080, ColorDepth = 24 },
				BotD = new BotD
				{
					Result = new BotDResult { Bot = true, BotKind = "automated" },
					Detection = "detected",
					Component = "component"
				}
			};

			var deviceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(deviceCode, message);

			// Assert
			result.Should().NotBeNull();
			result.NA.Should().Be("test-na");
			result.DeviceFPCode.Should().Be("test-device");
			result.AppVersion.Should().Be("1.0.0");
			result.TimeZone.Should().Be("UTC");
			result.UserAgent.Should().Be("test-ua");
			result.DeviceNo.Should().Be("test-id");
			result.HRef.Should().Be("https://test.com");
			result.CapturedDate.Should().Be(123456789);
			result.Screen.Should().NotBeNull();
			result.Screen.Width.Should().Be(1920);
			result.Screen.Height.Should().Be(1080);
			result.Screen.ColorDepth.Should().Be(24);
			result.BotD.Should().NotBeNull();
			result.BotD.Result.Should().NotBeNull();
			result.BotD.Result.Bot.Should().BeTrue();
			result.BotD.Result.BotKind.Should().Be("automated");
			result.BotD.Detection.Should().Be("detected");
			result.BotD.Component.Should().Be("component");
			result.InvalidDeviceId.Should().BeFalse();
		}

		[Fact]
		public void GetDeviceInfo_WithNullDeviceCode_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(null!, message);

			// Assert
			result.Should().NotBeNull();
			result.AppVersion.Should().BeEmpty();
			result.DeviceFPCode.Should().BeEmpty();
			result.DeviceNo.Should().BeEmpty();
			result.HRef.Should().BeEmpty();
			result.NA.Should().BeEmpty();
			result.TimeZone.Should().BeEmpty();
			result.UserAgent.Should().BeEmpty();
			result.CapturedDate.Should().Be(0);
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
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo("", message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithWhitespaceDeviceCode_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo("   ", message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithNullStringDeviceCode_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo("null", message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithNullStringDeviceCodeCaseInsensitive_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo("NULL", message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithInvalidBase64_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo("invalid-base64!", message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithInvalidJson_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var invalidJson = Convert.ToBase64String(Encoding.UTF8.GetBytes("{ invalid json }"));
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(invalidJson, message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithDeviceInfoHavingNullScreen_ShouldHandleGracefully()
		{
			// Arrange
			var deviceInfo = new DeviceInfo
			{
				NA = "test-na",
				DeviceCode = "test-device",
				AppVersion = "1.0.0",
				TimeZone = "UTC",
				UserAgent = "test-ua",
				DeviceId = "test-id",
				HRef = "https://test.com",
				CapturedDate = 123456789,
				Version = "2.0.0",
				Screen = null,
				BotD = null
			};

			var deviceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(deviceCode, message);

			// Assert
			result.Should().NotBeNull();
			result.Screen.Should().NotBeNull();
			result.Screen.Width.Should().Be(0);
			result.Screen.Height.Should().Be(0);
			result.Screen.ColorDepth.Should().Be(0);
			result.BotD.Should().NotBeNull();
			result.BotD.Result.Should().BeNull();
			result.BotD.Detection.Should().BeNull();
			result.BotD.Component.Should().BeNull();
		}

		[Fact]
		public void GetDeviceInfo_WithDeviceInfoHavingNullCapturedDate_ShouldUseCurrentTicks()
		{
			// Arrange
			var deviceInfo = new DeviceInfo
			{
				NA = "test-na",
				DeviceCode = "test-device",
				AppVersion = "1.0.0",
				TimeZone = "UTC",
				UserAgent = "test-ua",
				DeviceId = "test-id",
				HRef = "https://test.com",
				CapturedDate = null,
				Version = "2.0.0",
				Screen = new DeviceScreen { Width = 1920, Height = 1080, ColorDepth = 24 },
				BotD = new BotD()
			};

			var deviceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(deviceCode, message);

			// Assert
			result.Should().NotBeNull();
			result.CapturedDate.Should().BeGreaterThan(0);
			result.CapturedDate.Should().BeInRange(DateTime.Now.AddSeconds(-1).Ticks, DateTime.Now.AddSeconds(1).Ticks);
		}

		[Fact]
		public void GetDeviceInfo_WithDeviceInfoHavingNullBotD_ShouldHandleGracefully()
		{
			// Arrange
			var deviceInfo = new DeviceInfo
			{
				NA = "test-na",
				DeviceCode = "test-device",
				AppVersion = "1.0.0",
				TimeZone = "UTC",
				UserAgent = "test-ua",
				DeviceId = "test-id",
				HRef = "https://test.com",
				CapturedDate = 123456789,
				Version = "2.0.0",
				Screen = new DeviceScreen { Width = 1920, Height = 1080, ColorDepth = 24 },
				BotD = null
			};

			var deviceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
			var message = new MessageQueue();

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
		public void GetDeviceInfo_WithEmptyDeviceInfo_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var deviceInfo = new DeviceInfo();
			var deviceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(deviceCode, message);

			// Assert
			result.Should().NotBeNull();
			result.NA.Should().BeNull();
			result.DeviceFPCode.Should().BeNull();
			result.AppVersion.Should().BeNull();
			result.TimeZone.Should().BeNull();
			result.UserAgent.Should().BeNull();
			result.DeviceNo.Should().BeNull();
			result.HRef.Should().BeNull();
			result.CapturedDate.Should().BeGreaterThan(0); // Service uses current ticks when CapturedDate is null
			result.Version.Should().BeNull();
			result.InvalidDeviceId.Should().BeFalse();
		}

		[Fact]
		public void GetDeviceInfo_WithNullMessage_ShouldWorkNormally()
		{
			// Arrange
			var deviceInfo = new DeviceInfo
			{
				NA = "test-na",
				DeviceCode = "test-device",
				AppVersion = "1.0.0",
				TimeZone = "UTC",
				UserAgent = "test-ua",
				DeviceId = "test-id",
				HRef = "https://test.com",
				CapturedDate = 123456789,
				Version = "2.0.0",
				Screen = new DeviceScreen { Width = 1920, Height = 1080, ColorDepth = 24 },
				BotD = new BotD()
			};

			var deviceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));

			// Act
			var result = _service.GetDeviceInfo(deviceCode, null!);

			// Assert
			result.Should().NotBeNull();
			result.NA.Should().Be("test-na");
			result.DeviceFPCode.Should().Be("test-device");
		}

		[Theory]
		[InlineData("null")]
		[InlineData("NULL")]
		[InlineData("Null")]
		[InlineData("nUlL")]
		public void GetDeviceInfo_WithVariousNullStringCases_ShouldReturnEmptyDeviceInfo(string deviceCode)
		{
			// Arrange
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(deviceCode, message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithExceptionInDeserialization_ShouldReturnEmptyDeviceInfo()
		{
			// Arrange
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo("exception-trigger", message);

			// Assert
			result.Should().NotBeNull();
			result.InvalidDeviceId.Should().BeTrue();
		}

		[Fact]
		public void GetDeviceInfo_WithDeviceInfoHavingNullBotDResult_ShouldHandleGracefully()
		{
			// Arrange
			var deviceInfo = new DeviceInfo
			{
				NA = "test-na",
				DeviceCode = "test-device",
				AppVersion = "1.0.0",
				TimeZone = "UTC",
				UserAgent = "test-ua",
				DeviceId = "test-id",
				HRef = "https://test.com",
				CapturedDate = 123456789,
				Version = "2.0.0",
				Screen = new DeviceScreen { Width = 1920, Height = 1080, ColorDepth = 24 },
				BotD = new BotD { Result = null, Detection = "detected", Component = "component" }
			};

			var deviceCode = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(deviceInfo, JsonSerializerSettingsFactory.Instance)));
			var message = new MessageQueue();

			// Act
			var result = _service.GetDeviceInfo(deviceCode, message);

			// Assert
			result.Should().NotBeNull();
			result.BotD.Should().NotBeNull();
			result.BotD.Result.Should().BeNull();
			// The service creates a new BotDModel, so Detection and Component will be null
			result.BotD.Detection.Should().BeNull();
			result.BotD.Component.Should().BeNull();
		}
	}
}

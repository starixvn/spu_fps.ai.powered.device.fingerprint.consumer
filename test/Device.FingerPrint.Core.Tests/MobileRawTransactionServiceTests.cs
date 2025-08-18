using Device.FingerPrint.Core;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Moq;
using System;
using Xunit;

namespace Device.FingerPrint.Core.Tests
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
			_mockAppSettings.Setup(x => x.Environment).Returns("Test");
			_service = new MobileRawTransactionService(_mockUserAgentService.Object, _mockAppSettings.Object);
		}

		[Fact]
		public void TransformRawTransaction_WithValidIPv4_MapsAllFieldsAndIP()
		{
			var message = new MessageQueue
			{
				Account = "acc",
				Subscriber = "sub",
				SubscriberId = 123,
				TransDate = new DateTime(2024, 1, 2, 3, 4, 5, 678),
				Action = "login",
				ActionResult = "success",
				IP = " 127.0.0.1 ",
				Mobile = new MobileModel
				{
					MediaDRMCode = "mdrm",
					MachineCode = "machine",
					CustomCode = "custom",
					IsMobileAppTrans = true,
					IsEmulator = false,
					ModelName = "iPhone",
					Manufacturer = "Apple",
					Brand = "Apple",
					OS = "iOS",
					Timezone = "+07:00",
					SystemLanguage = "vi-VN",
					Country = "VN",
					SdkVersion = "17",
					DeviceDetails = "details"
				}
			};

			var result = _service.TransformRawTransaction(message);

			result.Should().NotBeNull();
			result!.LoginName.Should().Be("acc");
			result.SubscriberName.Should().Be("sub");
			result.SubscriberId.Should().Be(123);
			result.Action.Should().Be("login");
			result.ActionResult.Should().Be("success");
			result.MBDeviceCodeMediaDRM.Should().Be("mdrm");
			result.MBDeviceCodeMachine.Should().Be("machine");
			result.MBDeviceCodeTagging.Should().Be("custom");
			result.MBIsMobileApp.Should().BeTrue();
			result.MBIsEmulator.Should().BeFalse();
			result.MBModelName.Should().Be("iPhone");
			result.MBManufacturer.Should().Be("Apple");
			result.MBBrand.Should().Be("Apple");
			result.MBOS.Should().Be("iOS");
			result.MBTimezone.Should().Be("+07:00");
			result.MBLanguage.Should().Be("vi-VN");
			result.MBCountry.Should().Be("VN");
			result.MBSDKVersion.Should().Be("17");
			result.MBDeviceDetails.Should().Be("details");
			// TransTime formatted equality
			result.TransTime.ToString("yyyy-MM-dd HH:mm:ss.ffff").Should().Be(message.TransDate.ToString("yyyy-MM-dd HH:mm:ss.ffff"));
			// IP transform
			result.IP.Should().Be("127.0.0.1");
			var expectedIp = "127.0.0.1".Ip2Long();
			result.IPId.Should().Be(expectedIp.IpNumber);
			result.IsIPV6.Should().BeFalse();
			// Flagged code
			result.FlaggedCode.Should().Be(Flagged.Human.ToString());
		}

		[Fact]
		public void TransformRawTransaction_EmptyDeviceCodes_ReturnsEmptyDeviceFlag()
		{
			var message = new MessageQueue
			{
				Account = "acc",
				Subscriber = "sub",
				SubscriberId = 1,
				TransDate = DateTime.UtcNow,
				IP = "1.1.1.1",
				Mobile = new MobileModel
				{
					MediaDRMCode = null,
					MachineCode = "",
					CustomCode = null,
					IsMobileAppTrans = false
				}
			};

			var result = _service.TransformRawTransaction(message);

			result.Should().NotBeNull();
			result!.FlaggedCode.Should().Be(Flagged.EmptyDevice.ToString());
		}

		[Fact]
		public void TransformRawTransaction_WithIPv6_SetsIsIPv6True()
		{
			var ipv6 = "2001:0db8:85a3:0000:0000:8a2e:0370:7334";
			var message = new MessageQueue
			{
				Account = "acc",
				Subscriber = "sub",
				SubscriberId = 1,
				TransDate = DateTime.UtcNow,
				IP = ipv6,
				Mobile = new MobileModel { MachineCode = "a", CustomCode = "b", MediaDRMCode = "c" }
			};

			var result = _service.TransformRawTransaction(message);

			result.Should().NotBeNull();
			var expected = ipv6.Ip2Long();
			result!.IsIPV6.Should().BeTrue();
			result.IPId.Should().Be(expected.IpNumber);
		}

		[Fact]
		public void TransformRawTransaction_WithWhitespaceIP_TrimsIP()
		{
			var message = new MessageQueue
			{
				Account = "acc",
				Subscriber = "sub",
				SubscriberId = 1,
				TransDate = DateTime.UtcNow,
				IP = " 10.0.0.1  ",
				Mobile = new MobileModel { MachineCode = "x" }
			};

			var result = _service.TransformRawTransaction(message);

			result.Should().NotBeNull();
			result!.IP.Should().Be("10.0.0.1");
		}

		[Fact(Skip = "Depends on Fanex.Logging engine; skip to avoid external logging setup")]
		public void TransformRawTransaction_NullMobile_ReturnsNull()
		{
			var message = new MessageQueue
			{
				Account = "acc",
				Subscriber = "sub",
				SubscriberId = 1,
				TransDate = DateTime.UtcNow,
				IP = "1.1.1.1",
				Mobile = null!
			};

			var result = _service.TransformRawTransaction(message);
			result.Should().BeNull();
		}

		[Fact(Skip = "Depends on Fanex.Logging engine; skip to avoid external logging setup")]
		public void TransformRawTransaction_NullMessage_ReturnsNull()
		{
			MessageQueue msg = null!;
			var result = _service.TransformRawTransaction(msg);
			result.Should().BeNull();
		}
	}
}

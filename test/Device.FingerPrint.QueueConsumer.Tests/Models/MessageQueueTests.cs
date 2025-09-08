using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.TransactionAPI.Model;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    public class MessageQueueTests
    {
        [Fact]
        public void MessageQueue_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var message = new MessageQueue();
            var testDate = DateTime.Now;
            var subscriberDate = DateTime.Now.AddMinutes(5);

            // Act
            message.Account = "testAccount";
            message.Action = "login";
            message.ActionResult = "success";
            message.IP = "192.168.1.1";
            message.FakeIP = "10.0.0.1";
            message.Subscriber = "testSubscriber";
            message.SubscriberId = 123;
            message.TransDate = testDate;
            message.SubscriberDate = subscriberDate;
            message.Token = "testToken";
            message.Recaptcha = "recaptchaValue";
            message.SessionKey = "sessionKey123";
            message.BotD = "botDetection";
            message.WebRTCIP = "192.168.1.100";
            message.FPPatternCode01 = "pattern01";
            message.FPPatternCode02 = "pattern02";

            // Assert
            message.Account.Should().Be("testAccount");
            message.Action.Should().Be("login");
            message.ActionResult.Should().Be("success");
            message.IP.Should().Be("192.168.1.1");
            message.FakeIP.Should().Be("10.0.0.1");
            message.Subscriber.Should().Be("testSubscriber");
            message.SubscriberId.Should().Be(123);
            message.TransDate.Should().Be(testDate);
            message.SubscriberDate.Should().Be(subscriberDate);
            message.Token.Should().Be("testToken");
            message.Recaptcha.Should().Be("recaptchaValue");
            message.SessionKey.Should().Be("sessionKey123");
            message.BotD.Should().Be("botDetection");
            message.WebRTCIP.Should().Be("192.168.1.100");
            message.FPPatternCode01.Should().Be("pattern01");
            message.FPPatternCode02.Should().Be("pattern02");
        }

        [Fact]
        public void MessageQueue_DeviceProperty_ShouldBeSettableAndGettable()
        {
            // Arrange
            var message = new MessageQueue();
            var device = new DeviceModel { Di = "device123" };

            // Act
            message.Device = device;

            // Assert
            message.Device.Should().NotBeNull();
            message.Device.Di.Should().Be("device123");
        }

        [Fact]
        public void MessageQueue_MobileProperty_ShouldBeSettableAndGettable()
        {
            // Arrange
            var message = new MessageQueue();
            var mobile = new MobileModel
            {
                IsMobileAppTrans = true,
                IsEmulator = false,
                ModelName = "iPhone 12"
            };

            // Act
            message.Mobile = mobile;

            // Assert
            message.Mobile.Should().NotBeNull();
            message.Mobile.IsMobileAppTrans.Should().BeTrue();
            message.Mobile.IsEmulator.Should().BeFalse();
            message.Mobile.ModelName.Should().Be("iPhone 12");
        }

        [Fact]
        public void MessageQueue_FingerprintProperty_ShouldHaveDefaultValue()
        {
            // Arrange & Act
            var message = new MessageQueue();

            // Assert
            message.Fingerprint.Should().NotBeNull();
            message.Fingerprint.Should().BeOfType<APFingerprintModel>();
        }

        [Fact]
        public void MessageQueue_JsonSerialization_ShouldWork()
        {
            // Arrange
            var message = new MessageQueue
            {
                Account = "testAccount",
                Action = "login",
                TransDate = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                Device = new DeviceModel { Di = "device123" }
            };

            // Act
            var json = JsonConvert.SerializeObject(message);
            var deserializedMessage = JsonConvert.DeserializeObject<MessageQueue>(json);

            // Assert
            deserializedMessage.Should().NotBeNull();
            deserializedMessage.Account.Should().Be("testAccount");
            deserializedMessage.Action.Should().Be("login");
            deserializedMessage.TransDate.Should().Be(new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            deserializedMessage.Device.Should().NotBeNull();
            deserializedMessage.Device.Di.Should().Be("device123");
        }

        [Fact]
        public void MessageQueue_NullableProperties_ShouldAcceptNullValues()
        {
            // Arrange & Act
            var message = new MessageQueue
            {
                Account = null,
                Action = null,
                Device = null,
                Mobile = null,
                IP = null
            };

            // Assert
            message.Account.Should().BeNull();
            message.Action.Should().BeNull();
            message.Device.Should().BeNull();
            message.Mobile.Should().BeNull();
            message.IP.Should().BeNull();
        }
    }
}

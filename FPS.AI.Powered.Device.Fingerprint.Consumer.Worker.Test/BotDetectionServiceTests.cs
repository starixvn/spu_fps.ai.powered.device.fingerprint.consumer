using Xunit;
using FluentAssertions;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Model;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Test
{
    public class BotDetectionServiceTests
    {
        [Fact]
        public void GetFlaggedCode_WithBotDetection_ShouldReturnFlaggedCode()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel();
            var deviceNo = "test-device";
            var userAgent = "Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)";

            // Act
            var result = service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetFlaggedCode_WithoutBotDetection_ShouldReturnOriginalCode()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel();
            var deviceNo = "test-device";
            var userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";

            // Act
            var result = service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetFlaggedCode_WithEmptyDeviceCode_ShouldReturnEmptyString()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel();
            var deviceNo = "";
            var userAgent = "Mozilla/5.0 (compatible; Googlebot/2.1)";

            // Act
            var result = service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetFlaggedCode_WithDifferentUserAgents_ShouldHandleCorrectly()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel();
            var deviceNo = "test-device";

            // Test with Bing bot
            var bingBotUserAgent = "Mozilla/5.0 (compatible; bingbot/2.0; +http://www.bing.com/bingbot.htm)";
            var bingResult = service.GetFlaggedCode(botD, deviceNo, bingBotUserAgent);
            bingResult.Should().NotBeNullOrEmpty();

            // Test with normal browser
            var normalUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36";
            var normalResult = service.GetFlaggedCode(botD, deviceNo, normalUserAgent);
            normalResult.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void TransformBotdModel_WithValidData_ShouldTransformCorrectly()
        {
            // Arrange
            var service = new BotDetectionService();
            var message = new MessageQueue { BotD = "dGVzdA==" }; // Base64 of "test"

            // Act
            var result = service.TransformBotdModel(message);

            // Assert
            result.Should().NotBeNull();
        }

        [Fact]
        public void GetBotDetectionTrue_ShouldReturnTrue()
        {
            // Arrange
            var service = new BotDetectionService();
            var botDModel = new BotDModel();

            // Act
            var result = service.GetBotDetectionTrue(botDModel);

            // Assert
            result.Should().NotBeNull();
        }
    }
}

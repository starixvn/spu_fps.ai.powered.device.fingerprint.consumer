using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Services
{
    public class BotDetectionServiceTests
    {
        [Fact]
        public void GetFlaggedCode_WithBotDetection_ShouldReturnBotKind()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel
            {
                Result = new BotDResultModel
                {
                    Bot = true,
                    BotKind = "automated"
                }
            };
            var deviceNo = "test-device";
            var userAgent = "Mozilla/5.0";

            // Act
            var result = service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("automated");
        }

        [Fact]
        public void GetFlaggedCode_WithNoBotDetection_ShouldReturnZero()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel
            {
                Result = new BotDResultModel
                {
                    Bot = false
                }
            };
            var deviceNo = "test-device";
            var userAgent = "Mozilla/5.0";

            // Act
            var result = service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("Human");
        }

        [Fact]
        public void GetFlaggedCode_WithEmptyDeviceNo_ShouldReturnTwo()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel
            {
                Result = new BotDResultModel
                {
                    Bot = false
                }
            };
            var deviceNo = "";
            var userAgent = "Mozilla/5.0";

            // Act
            var result = service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("EmptyDevice");
        }

        [Fact]
        public void GetFlaggedCode_WithEmptyUserAgent_ShouldReturnThree()
        {
            // Arrange
            var service = new BotDetectionService();
            var botD = new BotDModel
            {
                Result = new BotDResultModel
                {
                    Bot = false
                }
            };
            var deviceNo = "test-device";
            var userAgent = "";

            // Act
            var result = service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("EmptyUserAgent");
        }
    }
}

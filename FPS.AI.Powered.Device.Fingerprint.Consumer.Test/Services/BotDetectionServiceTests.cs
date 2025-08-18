using Device.FingerPrint.Core.Encoders;
using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Services
{
    public class BotDetectionServiceTests
    {
        private readonly BotDetectionService _service;

        public BotDetectionServiceTests()
        {
            _service = new BotDetectionService();
        }

        [Fact]
        public void GetFlaggedCode_WithBotDetection_ShouldReturnBotKind()
        {
            // Arrange
            var botD = new BotDModel
            {
                Result = new BotDResult
                {
                    Bot = true,
                    BotKind = "automated"
                }
            };
            var deviceNo = "test-device";
            var userAgent = "Mozilla/5.0";

            // Act
            var result = _service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("automated");
        }

        [Fact]
        public void GetFlaggedCode_WithNoBotDetection_ShouldReturnBrowserlessFlag()
        {
            // Arrange
            var botD = new BotDModel
            {
                Result = new BotDResult
                {
                    Bot = false
                }
            };
            var deviceNo = "test-device";
            var userAgent = "Mozilla/5.0";

            // Act
            var result = _service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("0"); // Not flagged
        }

        [Fact]
        public void GetFlaggedCode_WithEmptyDeviceNo_ShouldReturnEmptyDeviceFlag()
        {
            // Arrange
            var botD = new BotDModel
            {
                Result = new BotDResult
                {
                    Bot = false
                }
            };
            var deviceNo = "";
            var userAgent = "Mozilla/5.0";

            // Act
            var result = _service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("2"); // EmptyDevice flag
        }

        [Fact]
        public void TransformBotdModel_WithValidBotDData_ShouldReturnBotDModel()
        {
            // Arrange
            var botDData = new BotDModel
            {
                IsVer3 = true,
                Detection = "{\"test\":{\"bot\":true}}"
            };
            var encodedBotD = Base64.Encode(JsonConvert.SerializeObject(botDData));

            var message = new MessageQueue
            {
                BotD = encodedBotD
            };

            // Act
            var result = _service.TransformBotdModel(message);

            // Assert
            result.Should().NotBeNull();
            result.IsVer3.Should().BeTrue();
            result.Detection.Should().Be("{\"test\":{\"bot\":true}}");
        }

        [Fact]
        public void TransformBotdModel_WithInvalidBotDData_ShouldReturnEmptyBotDModel()
        {
            // Arrange
            var message = new MessageQueue
            {
                BotD = "invalid-base64-data"
            };

            // Act
            var result = _service.TransformBotdModel(message);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BotDModel>();
        }

        [Fact]
        public void TransformBotdModel_WithEmptyBotD_ShouldReturnEmptyBotDModel()
        {
            // Arrange
            var message = new MessageQueue
            {
                BotD = ""
            };

            // Act
            var result = _service.TransformBotdModel(message);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BotDModel>();
        }

        [Fact]
        public void GetBotDetectionTrue_WithValidDetection_ShouldReturnBotKeys()
        {
            // Arrange
            var botDModel = new BotDModel
            {
                Detection = "{\"automated\":{\"bot\":true},\"manual\":{\"bot\":false},\"headless\":{\"bot\":true}}"
            };

            // Act
            var result = _service.GetBotDetectionTrue(botDModel);

            // Assert
            result.Should().Be("automated,headless");
        }

        [Fact]
        public void GetBotDetectionTrue_WithNoBots_ShouldReturnEmptyString()
        {
            // Arrange
            var botDModel = new BotDModel
            {
                Detection = "{\"manual\":{\"bot\":false},\"legitimate\":{\"bot\":false}}"
            };

            // Act
            var result = _service.GetBotDetectionTrue(botDModel);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetBotDetectionTrue_WithInvalidJson_ShouldReturnEmptyString()
        {
            // Arrange
            var botDModel = new BotDModel
            {
                Detection = "invalid-json"
            };

            // Act
            var result = _service.GetBotDetectionTrue(botDModel);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetBotDetectionTrue_WithNullDetection_ShouldReturnEmptyString()
        {
            // Arrange
            var botDModel = new BotDModel
            {
                Detection = null
            };

            // Act
            var result = _service.GetBotDetectionTrue(botDModel);

            // Assert
            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) HeadlessChrome/91.0.4472.124 Safari/537.36", "1")] // Headless
        [InlineData("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36", "0")] // Normal
        [InlineData("", "3")] // Empty UserAgent
        public void GetFlaggedCode_WithDifferentUserAgents_ShouldReturnCorrectFlags(string userAgent, string expectedFlag)
        {
            // Arrange
            var botD = new BotDModel
            {
                Result = new BotDResult
                {
                    Bot = false
                }
            };
            var deviceNo = "test-device";

            // Act
            var result = _service.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be(expectedFlag);
        }
    }
}

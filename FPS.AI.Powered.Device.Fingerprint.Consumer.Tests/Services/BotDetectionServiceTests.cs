using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Tests.Services
{
    public class BotDetectionServiceTests
    {
        private readonly BotDetectionService _botDetectionService;

        public BotDetectionServiceTests()
        {
            _botDetectionService = new BotDetectionService();
        }

        [Fact]
        public void GetFlaggedCode_WhenBotDIsNull_ShouldReturnBrowserlessFlag()
        {
            // Arrange
            BotDModel? botD = null;
            string deviceNo = "test-device";
            string userAgent = "Mozilla/5.0";

            // Act
            var result = _botDetectionService.GetFlaggedCode(botD!, deviceNo, userAgent);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetFlaggedCode_WhenBotDHasBotTrue_ShouldReturnBotKind()
        {
            // Arrange
            var botD = new BotDModel
            {
                Result = new BotDResultModel
                {
                    Bot = true,
                    BotKind = "headless"
                }
            };
            string deviceNo = "test-device";
            string userAgent = "Mozilla/5.0";

            // Act
            var result = _botDetectionService.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().Be("headless");
        }

        [Fact]
        public void GetFlaggedCode_WhenBotDHasBotFalse_ShouldReturnBrowserlessFlag()
        {
            // Arrange
            var botD = new BotDModel
            {
                Result = new BotDResultModel
                {
                    Bot = false,
                    BotKind = ""
                }
            };
            string deviceNo = "test-device";
            string userAgent = "Mozilla/5.0";

            // Act
            var result = _botDetectionService.GetFlaggedCode(botD, deviceNo, userAgent);

            // Assert
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetFlaggedCode_WhenEmptyDeviceNo_ShouldReturnEmptyDeviceFlag()
        {
            // Arrange
            BotDModel? botD = null;
            string deviceNo = "";
            string userAgent = "Mozilla/5.0";

            // Act
            var result = _botDetectionService.GetFlaggedCode(botD!, deviceNo, userAgent);

            // Assert
            result.Should().Be("EmptyDevice");
        }




    }
}

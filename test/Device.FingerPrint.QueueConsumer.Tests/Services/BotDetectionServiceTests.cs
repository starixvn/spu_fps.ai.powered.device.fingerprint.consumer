using Xunit;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Model;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class BotDetectionServiceTests
    {
        [Fact]
        public void GetFlaggedCode_Should_Return_BotKind_When_Bot_True()
        {
            var service = new BotDetectionService();
            var botD = new BotDModel { Result = new BotDResultModel { Bot = true, BotKind = "search-engine" } };
            var result = service.GetFlaggedCode(botD, "deviceNo", "userAgent");
            Assert.Equal("search-engine", result);
        }

        [Fact]
        public void GetFlaggedCode_Should_Return_Browserless_When_BotKind_Empty()
        {
            var service = new BotDetectionService();
            var botD = new BotDModel { Result = new BotDResultModel { Bot = false, BotKind = "" } };
            var result = service.GetFlaggedCode(botD, "", "headless-chrome");
            Assert.Equal("Headless", result);
        }

        [Fact]
        public void TransformBotdModel_Should_Return_BotDModel_When_Valid_Base64_Json()
        {
            var service = new BotDetectionService();
            var botDJson = "{\"Result\":{\"Bot\":true,\"BotKind\":\"search-engine\"}}";
            var botDBase64 = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(botDJson));
            var message = new MessageQueue { BotD = botDBase64 };
            var result = service.TransformBotdModel(message);
            Assert.True(result.Result.Bot);
            Assert.Equal("search-engine", result.Result.BotKind);
        }

        [Fact]
        public void GetBotDetectionTrue_Should_Return_Keys_With_Bot_True()
        {
            var service = new BotDetectionService();
            var detectionJson = "{\"google\":{\"bot\":true},\"bing\":{\"bot\":false},\"yahoo\":{\"bot\":true}}";
            var botDModel = new BotDModel { Detection = detectionJson };
            var result = service.GetBotDetectionTrue(botDModel);
            Assert.Contains("google", result);
            Assert.Contains("yahoo", result);
            Assert.DoesNotContain("bing", result);
        }
    }
}

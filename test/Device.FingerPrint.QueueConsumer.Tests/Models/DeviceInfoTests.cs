using Xunit;
using Device.FingerPrint.QueueConsumer.Models;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    public class DeviceInfoTests
    {
        [Fact]
        public void Should_Create_DeviceInfo_Instance()
        {
            var deviceInfo = new DeviceInfo();
            Assert.NotNull(deviceInfo);
        }

        [Fact]
        public void Should_Set_And_Get_Properties()
        {
            var screen = new DeviceScreen { Width = 1920, Height = 1080 };
            var botD = new BotD { Detection = "Ver3", Component = "Script", Result = new BotDResult { Bot = true, BotKind = "search-engine" } };
            var deviceInfo = new DeviceInfo
            {
                NA = "N/A",
                DeviceCode = "ABC123",
                AppVersion = "1.0.0",
                TimeZone = "UTC+7",
                UserAgent = "Mozilla/5.0",
                Screen = screen,
                DeviceId = "dev-001",
                HRef = "http://example.com",
                CapturedDate = 1690000000,
                Version = "v1",
                BotD = botD
            };
            Assert.Equal("N/A", deviceInfo.NA);
            Assert.Equal("ABC123", deviceInfo.DeviceCode);
            Assert.Equal("1.0.0", deviceInfo.AppVersion);
            Assert.Equal("UTC+7", deviceInfo.TimeZone);
            Assert.Equal("Mozilla/5.0", deviceInfo.UserAgent);
            Assert.Equal(screen, deviceInfo.Screen);
            Assert.Equal("dev-001", deviceInfo.DeviceId);
            Assert.Equal("http://example.com", deviceInfo.HRef);
            Assert.Equal(1690000000, deviceInfo.CapturedDate);
            Assert.Equal("v1", deviceInfo.Version);
            Assert.Equal(botD, deviceInfo.BotD);
        }
    }
}

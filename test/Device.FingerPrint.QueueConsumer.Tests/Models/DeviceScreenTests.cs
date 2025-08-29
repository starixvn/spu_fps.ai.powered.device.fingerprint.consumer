using Xunit;
using Device.FingerPrint.QueueConsumer.Models;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    public class DeviceScreenTests
    {
        [Fact]
        public void Should_Create_DeviceScreen_Instance()
        {
            var screen = new DeviceScreen();
            Assert.NotNull(screen);
        }

        [Fact]
        public void Should_Set_And_Get_Properties()
        {
            var screen = new DeviceScreen
            {
                Width = 1920,
                Height = 1080,
                ColorDepth = 32
            };
            Assert.Equal(1920, screen.Width);
            Assert.Equal(1080, screen.Height);
            Assert.Equal(32, screen.ColorDepth);
        }
    }
}

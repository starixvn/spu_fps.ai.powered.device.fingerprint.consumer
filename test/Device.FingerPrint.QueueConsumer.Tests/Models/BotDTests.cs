using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    using Xunit;
    using Device.FingerPrint.QueueConsumer.Models;

    public class BotDTests
    {
        [Fact]
        public void Should_Create_BotD_Instance()
        {
            // Arrange
            var botD = new BotD();

            // Assert
            Assert.NotNull(botD);
        }

        [Fact]
        public void Should_Set_And_Get_Properties()
        {
            // Arrange
            var botDResult = new BotDResult { Bot = true, BotKind = "search-engine" };
            var botD = new BotD
            {
                Result = botDResult,
                Detection = "Ver3",
                Component = "Script"
            };

            // Assert
            Assert.Equal(botDResult, botD.Result);
            Assert.Equal("Ver3", botD.Detection);
            Assert.Equal("Script", botD.Component);
        }
    }
}

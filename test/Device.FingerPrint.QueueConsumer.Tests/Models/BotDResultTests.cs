using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    using Xunit;
    using Device.FingerPrint.QueueConsumer.Models;

    public class BotDResultTests
    {
        [Fact]
        public void Should_Create_BotDResult_Instance()
        {
            // Arrange
            var botDResult = new BotDResult();

            // Assert
            Assert.NotNull(botDResult);
        }

        [Fact]
        public void Should_Set_And_Get_Properties()
        {
            // Arrange
            var botDResult = new BotDResult();
            botDResult.Bot = true;
            botDResult.BotKind = "search-engine";

            // Assert
            Assert.True(botDResult.Bot);
            Assert.Equal("search-engine", botDResult.BotKind);
        }
    }
}

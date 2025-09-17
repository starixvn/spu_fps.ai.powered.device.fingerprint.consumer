using Device.FingerPrint.Core;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class JsonSerializerSettingsFactoryTests
    {
        [Fact]
        public void Instance_IsNotNull_AndHasCamelCase()
        {
            var settings = JsonSerializerSettingsFactory.Instance;
            settings.Should().NotBeNull();

            var json = JsonConvert.SerializeObject(new { MyValue = 1 }, settings);
            json.Should().Contain("myValue");
        }
    }
}




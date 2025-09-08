using Device.FingerPrint.Core;
using FluentAssertions;
using Xunit;

namespace Device.FingerPrint.QueueConsumer.Tests.Core
{
    public class JsonSerializerSettingsFactoryTests
    {
        [Fact]
        public void Instance_ShouldReturnSameInstance()
        {
            // Arrange & Act
            var instance1 = JsonSerializerSettingsFactory.Instance;
            var instance2 = JsonSerializerSettingsFactory.Instance;

            // Assert
            instance1.Should().NotBeNull();
            instance2.Should().NotBeNull();
            instance1.Should().BeSameAs(instance2); // Singleton pattern
        }

        [Fact]
        public void Instance_ShouldHaveCamelCaseNaming()
        {
            // Arrange & Act
            var settings = JsonSerializerSettingsFactory.Instance;

            // Assert
            settings.ContractResolver.Should().NotBeNull();
            settings.ContractResolver.GetType().Name.Should().Contain("CamelCase");
        }

        [Fact]
        public void Instance_ShouldBeConfiguredProperly()
        {
            // Arrange & Act
            var settings = JsonSerializerSettingsFactory.Instance;

            // Assert
            settings.Should().NotBeNull();
            settings.ContractResolver.Should().NotBeNull();
        }
    }
}

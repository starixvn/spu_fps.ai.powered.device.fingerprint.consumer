using Device.FingerPrint.Core.Services;
using Device.FingerPrint.QueueConsumer;
using Newtonsoft.Json;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Tests.Core
{
    public class UtilitiesTests
    {
        private readonly ICrypto _mockCrypto;

        public UtilitiesTests()
        {
            _mockCrypto = Substitute.For<ICrypto>();
        }

        [Fact]
        public void GetMD5HashFromObject_WhenValidObject_ShouldReturnMD5Hash()
        {
            // Arrange
            var testObject = new { Name = "Test", Value = 123 };
            var serializedObject = JsonConvert.SerializeObject(testObject);
            _mockCrypto.GetMD5Hash(serializedObject).Returns("test-hash-123");

            // Act
            var result = Utilities.GetMD5HashFromObject(_mockCrypto, testObject);

            // Assert
            result.Should().Be("test-hash-123");
            _mockCrypto.Received(1).GetMD5Hash(serializedObject);
        }

        [Fact]
        public void GetMD5HashFromObject_WhenNullObject_ShouldReturnMD5HashOfNull()
        {
            // Arrange
            object? testObject = null;
            var serializedObject = JsonConvert.SerializeObject(testObject);
            _mockCrypto.GetMD5Hash(serializedObject).Returns("null-hash");

            // Act
            var result = Utilities.GetMD5HashFromObject(_mockCrypto, testObject);

            // Assert
            result.Should().Be("null-hash");
            _mockCrypto.Received(1).GetMD5Hash(serializedObject);
        }

        [Fact]
        public void IsValidJson_WhenValidJsonObject_ShouldReturnTrue()
        {
            // Arrange
            string json = "{\"name\":\"test\",\"value\":123}";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WhenValidJsonArray_ShouldReturnTrue()
        {
            // Arrange
            string json = "[1,2,3,{\"name\":\"test\"}]";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WhenInvalidJson_ShouldReturnFalse()
        {
            // Arrange
            string json = "invalid json string";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidJson_WhenEmptyString_ShouldReturnFalse()
        {
            // Arrange
            string json = "";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidJson_WhenWhitespaceString_ShouldReturnFalse()
        {
            // Arrange
            string json = "   ";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidJson_WhenJsonWithWhitespace_ShouldReturnTrue()
        {
            // Arrange
            string json = "  {\"name\":\"test\"}  ";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WhenPartialJsonObject_ShouldReturnFalse()
        {
            // Arrange
            string json = "{\"name\":\"test\"";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidJson_WhenPartialJsonArray_ShouldReturnFalse()
        {
            // Arrange
            string json = "[1,2,3";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidJson_WhenNestedJsonObject_ShouldReturnTrue()
        {
            // Arrange
            string json = "{\"name\":\"test\",\"nested\":{\"value\":123,\"array\":[1,2,3]}}";

            // Act
            var result = Utilities.IsValidJson(json);

            // Assert
            result.Should().BeTrue();
        }
    }
}

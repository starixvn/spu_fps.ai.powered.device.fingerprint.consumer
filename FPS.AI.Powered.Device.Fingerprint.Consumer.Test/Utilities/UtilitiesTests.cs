using Device.FingerPrint.Core.Services;
using Device.FingerPrint.QueueConsumer;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Utilities
{
    public class UtilitiesTests
    {
        private readonly Mock<ICrypto> _mockCrypto;

        public UtilitiesTests()
        {
            _mockCrypto = new Mock<ICrypto>();
        }

        [Fact]
        public void GetMD5HashFromObject_WithValidObject_ShouldReturnMD5Hash()
        {
            // Arrange
            var testObject = new { Name = "Test", Value = 123 };
            var expectedJson = JsonConvert.SerializeObject(testObject);
            var expectedHash = "test-md5-hash";

            _mockCrypto.Setup(x => x.GetMD5Hash(expectedJson)).Returns(expectedHash);

            // Act
            var result = Utilities.GetMD5HashFromObject(_mockCrypto.Object, testObject);

            // Assert
            result.Should().Be(expectedHash);
            _mockCrypto.Verify(x => x.GetMD5Hash(expectedJson), Times.Once);
        }

        [Fact]
        public void GetMD5HashFromObject_WithNullObject_ShouldReturnMD5HashOfNull()
        {
            // Arrange
            object testObject = null;
            var expectedJson = JsonConvert.SerializeObject(testObject);
            var expectedHash = "null-md5-hash";

            _mockCrypto.Setup(x => x.GetMD5Hash(expectedJson)).Returns(expectedHash);

            // Act
            var result = Utilities.GetMD5HashFromObject(_mockCrypto.Object, testObject);

            // Assert
            result.Should().Be(expectedHash);
            _mockCrypto.Verify(x => x.GetMD5Hash(expectedJson), Times.Once);
        }

        [Fact]
        public void GetMD5HashFromObject_WithComplexObject_ShouldReturnMD5Hash()
        {
            // Arrange
            var testObject = new
            {
                Name = "Test",
                Values = new[] { 1, 2, 3 },
                Nested = new { Key = "Value" }
            };
            var expectedJson = JsonConvert.SerializeObject(testObject);
            var expectedHash = "complex-md5-hash";

            _mockCrypto.Setup(x => x.GetMD5Hash(expectedJson)).Returns(expectedHash);

            // Act
            var result = Utilities.GetMD5HashFromObject(_mockCrypto.Object, testObject);

            // Assert
            result.Should().Be(expectedHash);
            _mockCrypto.Verify(x => x.GetMD5Hash(expectedJson), Times.Once);
        }

        [Theory]
        [InlineData("{\"key\":\"value\"}", true)]
        [InlineData("{\"key\":\"value\", \"number\":123}", true)]
        [InlineData("{\"key\":\"value\", \"array\":[1,2,3]}", true)]
        [InlineData("[1,2,3]", true)]
        [InlineData("[\"a\",\"b\",\"c\"]", true)]
        [InlineData("{\"key\":\"value\"", false)]
        [InlineData("key:value}", false)]
        [InlineData("invalid json", false)]
        [InlineData("", false)]
        [InlineData("   ", false)]
        [InlineData("  {\"key\":\"value\"}  ", true)] // Should trim whitespace
        public void IsValidJson_WithVariousInputs_ShouldReturnExpectedResult(string input, bool expected)
        {
            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().Be(expected);
        }

        [Fact]
        public void IsValidJson_WithNullInput_ShouldReturnFalse()
        {
            // Arrange
            string input = null;

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidJson_WithNestedJson_ShouldReturnTrue()
        {
            // Arrange
            var input = "{\"key\":\"value\",\"nested\":{\"inner\":\"data\"}}";

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WithArrayOfObjects_ShouldReturnTrue()
        {
            // Arrange
            var input = "[{\"id\":1,\"name\":\"test\"},{\"id\":2,\"name\":\"test2\"}]";

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WithEmptyObject_ShouldReturnTrue()
        {
            // Arrange
            var input = "{}";

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WithEmptyArray_ShouldReturnTrue()
        {
            // Arrange
            var input = "[]";

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WithWhitespaceOnly_ShouldReturnFalse()
        {
            // Arrange
            var input = "   \n\t   ";

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsValidJson_WithLeadingTrailingWhitespace_ShouldReturnTrue()
        {
            // Arrange
            var input = "  {\"key\":\"value\"}  ";

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsValidJson_WithComplexNestedStructure_ShouldReturnTrue()
        {
            // Arrange
            var input = "{\"key\":\"value\",\"array\":[1,2,{\"nested\":\"data\"}],\"object\":{\"inner\":{\"deep\":\"value\"}}}";

            // Act
            var result = Utilities.IsValidJson(input);

            // Assert
            result.Should().BeTrue();
        }
    }
}

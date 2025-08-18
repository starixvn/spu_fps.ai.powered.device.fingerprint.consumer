using Device.FingerPrint.Core.Encoders;
using FluentAssertions;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class Base64Tests
    {
        [Theory]
        [InlineData("SGVsbG8=", "Hello")]
        [InlineData("VGVzdA==", "Test")]
        [InlineData("U3BhY2VzIG9rIQ==", "Spaces ok!")]
        public void Decode_ValidBase64_ReturnsPlainText(string input, string expected)
        {
            var result = Base64.Decode(input);
            result.Should().Be(expected);
        }

        [Fact]
        public void Decode_InvalidChars_IgnoresTrailingGarbage()
        {
            var result = Base64.Decode("SGVsbG8=###");
            result.Should().Be("Hello");
        }
    }
}



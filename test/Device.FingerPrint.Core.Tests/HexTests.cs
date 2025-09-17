using Device.FingerPrint.Core.Encoders;
using FluentAssertions;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class HexTests
    {
        [Theory]
        [InlineData("ABC", "414243")]
        [InlineData("hello", "68656c6c6f")]
        [InlineData("", "")]
        public void Encode_ReturnsLowerHex(string input, string expectedHex)
        {
            Hex.Encode(input).Should().Be(expectedHex);
        }

        [Theory]
        [InlineData("414243", "ABC")]
        [InlineData("68656c6c6f", "hello")]
        public void Decode_WorksForValidHex(string hex, string expected)
        {
            Hex.Decode(hex).Should().Be(expected);
        }

        [Fact]
        public void Decode_OddLengthHex_DoesNotThrow()
        {
            var act = () => Hex.Decode("414");
            act.Should().NotThrow();
        }
    }
}



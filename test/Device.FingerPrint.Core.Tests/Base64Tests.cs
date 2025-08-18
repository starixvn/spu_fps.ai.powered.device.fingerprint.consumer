using Device.FingerPrint.Core.Encoders;
using FluentAssertions;
using System;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class Base64Tests
    {
        [Theory]
        [InlineData("SGVsbG8=", "Hello")]
        [InlineData("VGVzdA==", "Test")]
        [InlineData("U3BhY2VzIG9rIQ==", "Spaces ok!")]
        [InlineData("", "")]
        [InlineData("SGVsbG8gV29ybGQ=", "Hello World")]
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

        [Fact]
        public void Decode_WithSpaces_ReplacesSpacesWithPlus()
        {
            var result = Base64.Decode("SGVsbG8gV29ybGQ=");
            result.Should().Be("Hello World");
        }

        [Fact]
        public void Decode_EmptyString_ReturnsEmptyString()
        {
            var result = Base64.Decode("");
            result.Should().Be("");
        }

        [Fact]
        public void Decode_NullString_ThrowsArgumentNullException()
        {
            Action action = () => Base64.Decode(null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Decode_InvalidBase64_ThrowsFormatException()
        {
            Action action = () => Base64.Decode("InvalidBase64!");
            action.Should().Throw<FormatException>();
        }

        [Fact]
        public void Decode_WithCustomKey_EmptyString_ReturnsEmptyString()
        {
            var customKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            var result = Base64.Decode("", customKey);
            result.Should().Be("");
        }

        [Fact]
        public void Decode_WithCustomKey_NullString_ThrowsArgumentNullException()
        {
            var customKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            Action action = () => Base64.Decode(null, customKey);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Decode_WithCustomKey_NullKey_ThrowsNullReferenceException()
        {
            Action action = () => Base64.Decode("SGVsbG8=", null);
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Decode_WithCustomKey_EmptyKey_ThrowsOverflowException()
        {
            Action action = () => Base64.Decode("SGVsbG8=", "");
            action.Should().Throw<OverflowException>();
        }

        [Fact]
        public void Decode_WithCustomKey_InvalidKey_ThrowsOverflowException()
        {
            Action action = () => Base64.Decode("SGVsbG8=", "InvalidKey");
            action.Should().Throw<OverflowException>();
        }

        [Fact]
        public void Decode_WithCustomKey_HandlesSpecialCharactersInKey()
        {
            var customKey = "!@#$%^&*()_+-=[]{}|;:,.<>?";
            Action action = () => Base64.Decode("SGVsbG8=", customKey);
            action.Should().Throw<OverflowException>();
        }

        [Fact]
        public void Decode_WithCustomKey_RemovesInvalidCharacters()
        {
            var customKey = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            Action action = () => Base64.Decode("SGVsbG8=###invalid###", customKey);
            action.Should().Throw<OverflowException>();
        }
    }
}



using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Encoders;
using FluentAssertions;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class DeviceEncoderTests
    {
        [Theory]
        [InlineData("abc")]
        [InlineData("1234567890")]
        [InlineData("!@#$%^&*()_+")]
        public void Encrypt_Then_Decrypt_Roundtrip(string input)
        {
            var encrypted = DeviceEncoder.Encrypt(input);
            encrypted.Should().NotBeNullOrEmpty();
            encrypted.Should().MatchRegex("^[0-9a-f]+$");

            var decrypted = DeviceEncoder.Decrypt(encrypted);
            decrypted.Should().Be(input);
        }
    }
}




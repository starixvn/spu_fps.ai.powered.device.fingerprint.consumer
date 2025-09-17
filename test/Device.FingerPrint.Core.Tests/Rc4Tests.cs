using Device.FingerPrint.Core.Encoders;
using FluentAssertions;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class Rc4Tests
    {
        [Theory]
        [InlineData("key", "hello")]
        [InlineData("another-key", "some text 123")] 
        public void Encrypt_Then_Decrypt_Roundtrip(string key, string plain)
        {
            var encrypted = Rc4.Encrypt(key, plain);
            encrypted.Should().NotBe(plain);

            var decrypted = Rc4.Decrypt(key, encrypted);
            decrypted.Should().Be(plain);
        }
    }
}




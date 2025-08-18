using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class CryptoTests
    {
        private static IAppSettings CreateAppSettings()
        {
            var inMemory = new Dictionary<string, string?>
            {
                ["AppConfig:Crypto:Password"] = "password",
                ["AppConfig:Crypto:IV"] = "c3eef06037e837f4ee390bda69c05023",
                ["AppConfig:Crypto:Salt"] = "234292a9fce031f2",
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory!)
                .Build();

            return new AppSettings(config);
        }

        [Fact]
        public void Encrypt_Empty_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.Encrypt(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_Then_Decrypt_Roundtrip()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var encrypted = crypto.Encrypt("hello");
            encrypted.Should().NotBeNullOrEmpty();

            var decrypted = crypto.Decrypt(encrypted);
            decrypted.Should().Be("hello");
        }

        [Fact]
        public void GetMD5Hash_Empty_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.GetMD5Hash(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_KnownValue()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.GetMD5Hash("abc").Should().Be("900150983cd24fb0d6963f7d28e17f72");
        }

        [Fact]
        public void Dispose_ShouldDisposeRijndael()
        {
            var crypto = new Crypto(CreateAppSettings());
            crypto.Dispose();
            
            // Should not throw when disposed
            crypto.Dispose();
        }
    }
}



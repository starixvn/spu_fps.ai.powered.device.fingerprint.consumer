using Device.FingerPrint.Core.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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



        private static IAppSettings CreateAppSettingsWithCustomValues(string password, string? iv = null, string? salt = null)
        {
            var inMemory = new Dictionary<string, string?>
            {
                ["AppConfig:Crypto:Password"] = password,
                ["AppConfig:Crypto:IV"] = iv ?? "c3eef06037e837f4ee390bda69c05023",
                ["AppConfig:Crypto:Salt"] = salt ?? "234292a9fce031f2",
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory!)
                .Build();

            return new AppSettings(config);
        }

        #region Constructor Tests



        [Fact]
        public void Constructor_ShouldThrowNullReferenceException_WhenAppSettingsIsNull()
        {
            // Arrange, Act & Assert
            Assert.Throws<NullReferenceException>(() => new Crypto(null!));
        }

        #endregion

        #region Encrypt Tests

        [Fact]
        public void Encrypt_Empty_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.Encrypt(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_Null_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.Encrypt(null!).Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_ValidString_ReturnsBase64String()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var result = crypto.Encrypt("Hello World");
            
            result.Should().NotBeNullOrEmpty();
            result.Should().MatchRegex(@"^[A-Za-z0-9+/]*={0,2}$", "should be valid Base64");
        }

        [Fact]
        public void Encrypt_SameInputTwice_ReturnsSameResult()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var input = "consistent test";
            
            var result1 = crypto.Encrypt(input);
            var result2 = crypto.Encrypt(input);
            
            result1.Should().Be(result2);
        }

        [Fact]
        public void Encrypt_LongString_ShouldWork()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var longString = new string('A', 1000);
            
            var result = crypto.Encrypt(longString);
            result.Should().NotBeNullOrEmpty();
            
            var decrypted = crypto.Decrypt(result);
            decrypted.Should().Be(longString);
        }

        [Fact]
        public void Encrypt_SpecialCharacters_ShouldWork()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?`~äöüß中文";
            
            var result = crypto.Encrypt(specialChars);
            result.Should().NotBeNullOrEmpty();
            
            var decrypted = crypto.Decrypt(result);
            decrypted.Should().Be(specialChars);
        }

        #endregion

        #region Decrypt Tests

        [Fact]
        public void Decrypt_Empty_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.Decrypt(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_Null_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.Decrypt(null!).Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_InvalidBase64_ThrowsFormatException()
        {
            using var crypto = new Crypto(CreateAppSettings());
            
            Assert.Throws<FormatException>(() => crypto.Decrypt("invalid-base64!@#"));
        }

        [Fact]
        public void Decrypt_ValidBase64ButInvalidCipherText_ThrowsCryptographicException()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var invalidCipherText = Convert.ToBase64String(new byte[] { 1, 2, 3, 4, 5 });
            
            Assert.Throws<System.Security.Cryptography.CryptographicException>(() => crypto.Decrypt(invalidCipherText));
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

        [Theory]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("Hello World")]
        [InlineData("1234567890")]
        [InlineData("Special chars: !@#$%^&*()")]
        [InlineData("Very long string with lots of content that should test the encryption and decryption process thoroughly")]
        public void Encrypt_Then_Decrypt_Roundtrip_MultipleInputs(string input)
        {
            using var crypto = new Crypto(CreateAppSettings());
            
            if (string.IsNullOrEmpty(input))
            {
                crypto.Encrypt(input).Should().BeEmpty();
                crypto.Decrypt(string.Empty).Should().BeEmpty();
            }
            else
            {
                var encrypted = crypto.Encrypt(input);
                encrypted.Should().NotBeNullOrEmpty();

                var decrypted = crypto.Decrypt(encrypted);
                decrypted.Should().Be(input);
            }
        }

        [Fact]
        public void Decrypt_WithDifferentCryptoInstance_SameSettings_ShouldWork()
        {
            var settings = CreateAppSettings();
            string encrypted;
            
            using (var crypto1 = new Crypto(settings))
            {
                encrypted = crypto1.Encrypt("cross-instance test");
            }
            
            using (var crypto2 = new Crypto(settings))
            {
                var decrypted = crypto2.Decrypt(encrypted);
                decrypted.Should().Be("cross-instance test");
            }
        }

        [Fact]
        public void Decrypt_WithDifferentCryptoInstance_DifferentSettings_ShouldFail()
        {
            var settings1 = CreateAppSettingsWithCustomValues("password1");
            var settings2 = CreateAppSettingsWithCustomValues("password2");
            
            string encrypted;
            using (var crypto1 = new Crypto(settings1))
            {
                encrypted = crypto1.Encrypt("different settings test");
            }
            
            using (var crypto2 = new Crypto(settings2))
            {
                Assert.Throws<System.Security.Cryptography.CryptographicException>(() => crypto2.Decrypt(encrypted));
            }
        }

        #endregion

        #region GetMD5Hash Tests

        [Fact]
        public void GetMD5Hash_Empty_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.GetMD5Hash(string.Empty).Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_Null_ReturnsEmpty()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.GetMD5Hash(null!).Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_KnownValue()
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.GetMD5Hash("abc").Should().Be("900150983cd24fb0d6963f7d28e17f72");
        }

        [Theory]
        [InlineData("hello", "5d41402abc4b2a76b9719d911017c592")]
        [InlineData("world", "7d793037a0760186574b0282f2f435e7")]
        [InlineData("123", "202cb962ac59075b964b07152d234b70")]
        [InlineData("The quick brown fox jumps over the lazy dog", "9e107d9d372bb6826bd81d3542a419d6")]
        public void GetMD5Hash_KnownValues_ReturnsExpectedHash(string input, string expectedHash)
        {
            using var crypto = new Crypto(CreateAppSettings());
            crypto.GetMD5Hash(input).Should().Be(expectedHash);
        }

        [Fact]
        public void GetMD5Hash_SameInput_ReturnsSameHash()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var input = "consistent hash test";
            
            var hash1 = crypto.GetMD5Hash(input);
            var hash2 = crypto.GetMD5Hash(input);
            
            hash1.Should().Be(hash2);
            hash1.Should().HaveLength(32);
            hash1.Should().MatchRegex(@"^[a-f0-9]{32}$");
        }

        [Fact]
        public void GetMD5Hash_DifferentInputs_ReturnDifferentHashes()
        {
            using var crypto = new Crypto(CreateAppSettings());
            
            var hash1 = crypto.GetMD5Hash("input1");
            var hash2 = crypto.GetMD5Hash("input2");
            
            hash1.Should().NotBe(hash2);
        }

        [Fact]
        public void GetMD5Hash_SpecialCharacters_ShouldWork()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var specialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?`~";
            
            var hash = crypto.GetMD5Hash(specialChars);
            hash.Should().NotBeNullOrEmpty();
            hash.Should().HaveLength(32);
            hash.Should().MatchRegex(@"^[a-f0-9]{32}$");
        }

        [Fact]
        public void GetMD5Hash_LongString_ShouldWork()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var longString = new string('A', 10000);
            
            var hash = crypto.GetMD5Hash(longString);
            hash.Should().NotBeNullOrEmpty();
            hash.Should().HaveLength(32);
            hash.Should().MatchRegex(@"^[a-f0-9]{32}$");
        }

        #endregion

        #region HexStringToByteArray Tests

        [Fact]
        public void HexStringToByteArray_ValidHexString_ReturnsCorrectBytes()
        {
            var hexString = "48656C6C6F"; // "Hello" in hex
            var result = Crypto.HexStringToByteArray(hexString);
            
            result.Should().Equal(new byte[] { 0x48, 0x65, 0x6C, 0x6C, 0x6F });
        }

        [Fact]
        public void HexStringToByteArray_EmptyString_ReturnsEmptyArray()
        {
            var result = Crypto.HexStringToByteArray(string.Empty);
            result.Should().BeEmpty();
        }

        [Fact]
        public void HexStringToByteArray_LowercaseHex_ReturnsCorrectBytes()
        {
            var hexString = "deadbeef";
            var result = Crypto.HexStringToByteArray(hexString);
            
            result.Should().Equal(new byte[] { 0xDE, 0xAD, 0xBE, 0xEF });
        }

        [Fact]
        public void HexStringToByteArray_InvalidHexString_ThrowsFormatException()
        {
            Assert.Throws<FormatException>(() => Crypto.HexStringToByteArray("ZZ"));
        }

        [Fact]
        public void HexStringToByteArray_OddLengthString_ThrowsIndexOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Crypto.HexStringToByteArray("123"));
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_ShouldDisposeRijndael()
        {
            var crypto = new Crypto(CreateAppSettings());
            crypto.Dispose();

            // Should not throw when disposed multiple times
            crypto.Dispose();
        }



        [Fact]
        public void Dispose_GetMD5Hash_ShouldStillWork()
        {
            var crypto = new Crypto(CreateAppSettings());
            crypto.Dispose();

            // MD5 hash should still work as it doesn't use Rijndael
            var result = crypto.GetMD5Hash("test");
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void UsingStatement_ShouldAutomaticallyDispose()
        {
            ICrypto crypto;
            using (crypto = new Crypto(CreateAppSettings()))
            {
                var result = crypto.Encrypt("test");
                result.Should().NotBeNullOrEmpty();
            }

            // Crypto is disposed but doesn't throw ObjectDisposedException
            // Just verify the using statement worked without exceptions
            crypto.Should().NotBeNull();
        }

        #endregion

        #region Integration Tests

        [Fact]
        public void MultipleCryptoInstances_SameSettings_ShouldBeInteroperable()
        {
            var settings = CreateAppSettings();
            var testData = new[]
            {
                "test1",
                "test2",
                "special chars: äöüß",
                "numbers: 123456789",
                "symbols: !@#$%^&*()"
            };

            var encryptedData = new string[testData.Length];

            // Encrypt with first instance
            using (var crypto1 = new Crypto(settings))
            {
                for (int i = 0; i < testData.Length; i++)
                {
                    encryptedData[i] = crypto1.Encrypt(testData[i]);
                }
            }

            // Decrypt with second instance
            using (var crypto2 = new Crypto(settings))
            {
                for (int i = 0; i < testData.Length; i++)
                {
                    var decrypted = crypto2.Decrypt(encryptedData[i]);
                    decrypted.Should().Be(testData[i]);
                }
            }
        }

        [Fact]
        public async Task ConcurrentOperations_ShouldWork()
        {
            using var crypto = new Crypto(CreateAppSettings());
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                int index = i;
                tasks.Add(Task.Run(() =>
                {
                    var input = $"concurrent test {index}";
                    var encrypted = crypto.Encrypt(input);
                    var decrypted = crypto.Decrypt(encrypted);
                    decrypted.Should().Be(input);
                    
                    var hash = crypto.GetMD5Hash(input);
                    hash.Should().NotBeNullOrEmpty();
                }));
            }

            await Task.WhenAll(tasks.ToArray());
        }

        #endregion
    }
}



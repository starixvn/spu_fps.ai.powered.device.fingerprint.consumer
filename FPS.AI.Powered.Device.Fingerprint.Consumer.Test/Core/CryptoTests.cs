using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using FluentAssertions;
using Moq;
using Xunit;

namespace FPS.AI.Powered.Device.Fingerprint.Test.Core
{
    public class CryptoTests
    {
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly Crypto _crypto;

        public CryptoTests()
        {
            _mockAppSettings = new Mock<IAppSettings>();
            _mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            _mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            _mockAppSettings.Setup(x => x.CryptoPassword).Returns("test-password");

            _crypto = new Crypto(_mockAppSettings.Object);
        }

        [Fact]
        public void Encrypt_WithValidText_ShouldReturnEncryptedString()
        {
            // Arrange
            var plainText = "Hello, World!";

            // Act
            var result = _crypto.Encrypt(plainText);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().NotBe(plainText);
            result.Should().NotContain(plainText);
        }

        [Fact]
        public void Encrypt_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var plainText = "";

            // Act
            var result = _crypto.Encrypt(plainText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_WithNullString_ShouldReturnEmptyString()
        {
            // Arrange
            string plainText = null;

            // Act
            var result = _crypto.Encrypt(plainText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_WithValidEncryptedText_ShouldReturnOriginalText()
        {
            // Arrange
            var originalText = "Hello, World!";
            var encryptedText = _crypto.Encrypt(originalText);

            // Act
            var result = _crypto.Decrypt(encryptedText);

            // Assert
            result.Should().Be(originalText);
        }

        [Fact]
        public void Decrypt_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var encryptedText = "";

            // Act
            var result = _crypto.Decrypt(encryptedText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_WithNullString_ShouldReturnEmptyString()
        {
            // Arrange
            string encryptedText = null;

            // Act
            var result = _crypto.Decrypt(encryptedText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void EncryptDecrypt_WithSpecialCharacters_ShouldWorkCorrectly()
        {
            // Arrange
            var originalText = "Hello! @#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var encryptedText = _crypto.Encrypt(originalText);
            var decryptedText = _crypto.Decrypt(encryptedText);

            // Assert
            decryptedText.Should().Be(originalText);
        }

        [Fact]
        public void EncryptDecrypt_WithUnicodeCharacters_ShouldWorkCorrectly()
        {
            // Arrange
            var originalText = "Hello, 世界! Привет! مرحبا!";

            // Act
            var encryptedText = _crypto.Encrypt(originalText);
            var decryptedText = _crypto.Decrypt(encryptedText);

            // Assert
            decryptedText.Should().Be(originalText);
        }

        [Fact]
        public void EncryptDecrypt_WithLongText_ShouldWorkCorrectly()
        {
            // Arrange
            var originalText = new string('A', 1000);

            // Act
            var encryptedText = _crypto.Encrypt(originalText);
            var decryptedText = _crypto.Decrypt(encryptedText);

            // Assert
            decryptedText.Should().Be(originalText);
        }

        [Fact]
        public void EncryptDecrypt_WithNumbers_ShouldWorkCorrectly()
        {
            // Arrange
            var originalText = "1234567890";

            // Act
            var encryptedText = _crypto.Encrypt(originalText);
            var decryptedText = _crypto.Decrypt(encryptedText);

            // Assert
            decryptedText.Should().Be(originalText);
        }

        [Fact]
        public void GetMD5Hash_WithValidString_ShouldReturnMD5Hash()
        {
            // Arrange
            var input = "Hello, World!";

            // Act
            var result = _crypto.GetMD5Hash(input);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(32); // MD5 hash is always 32 characters
            result.Should().MatchRegex("^[a-f0-9]{32}$"); // Should be hexadecimal
        }

        [Fact]
        public void GetMD5Hash_WithEmptyString_ShouldReturnEmptyString()
        {
            // Arrange
            var input = "";

            // Act
            var result = _crypto.GetMD5Hash(input);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_WithNullString_ShouldReturnEmptyString()
        {
            // Arrange
            string input = null;

            // Act
            var result = _crypto.GetMD5Hash(input);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_WithSameInput_ShouldReturnSameHash()
        {
            // Arrange
            var input = "Test String";

            // Act
            var result1 = _crypto.GetMD5Hash(input);
            var result2 = _crypto.GetMD5Hash(input);

            // Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public void GetMD5Hash_WithDifferentInputs_ShouldReturnDifferentHashes()
        {
            // Arrange
            var input1 = "Test String 1";
            var input2 = "Test String 2";

            // Act
            var result1 = _crypto.GetMD5Hash(input1);
            var result2 = _crypto.GetMD5Hash(input2);

            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void GetMD5Hash_WithSpecialCharacters_ShouldReturnValidHash()
        {
            // Arrange
            var input = "Hello! @#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var result = _crypto.GetMD5Hash(input);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(32);
            result.Should().MatchRegex("^[a-f0-9]{32}$");
        }

        [Fact]
        public void GetMD5Hash_WithUnicodeCharacters_ShouldReturnValidHash()
        {
            // Arrange
            var input = "Hello, 世界! Привет! مرحبا!";

            // Act
            var result = _crypto.GetMD5Hash(input);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(32);
            result.Should().MatchRegex("^[a-f0-9]{32}$");
        }

        [Fact]
        public void GetMD5Hash_WithLongString_ShouldReturnValidHash()
        {
            // Arrange
            var input = new string('A', 1000);

            // Act
            var result = _crypto.GetMD5Hash(input);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(32);
            result.Should().MatchRegex("^[a-f0-9]{32}$");
        }

        [Theory]
        [InlineData("Hello, World!", "65a8e27d8879283831b664bd8b7f0ad4")]
        [InlineData("", "")]
        [InlineData("test", "098f6bcd4621d373cade4e832627b4f6")]
        [InlineData("123456", "e10adc3949ba59abbe56e057f20f883e")]
        public void GetMD5Hash_WithKnownInputs_ShouldReturnExpectedHashes(string input, string expectedHash)
        {
            // Act
            var result = _crypto.GetMD5Hash(input);

            // Assert
            result.Should().Be(expectedHash);
        }

        [Fact]
        public void HexStringToByteArray_WithValidHexString_ShouldReturnCorrectBytes()
        {
            // Arrange
            var hexString = "c3eef06037e837f4ee390bda69c05023";

            // Act
            var result = Crypto.HexStringToByteArray(hexString);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(16); // 32 hex characters = 16 bytes
        }

        [Fact]
        public void HexStringToByteArray_WithEmptyString_ShouldReturnEmptyArray()
        {
            // Arrange
            var hexString = "";

            // Act
            var result = Crypto.HexStringToByteArray(hexString);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().Be(0);
        }
    }
}

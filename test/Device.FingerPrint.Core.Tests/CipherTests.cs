using System;
using System.Linq;
using Device.FingerPrint.Core;
using FluentAssertions;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class CipherTests
    {
        [Fact]
        public void Encrypt_WithValidString_ShouldReturnEncryptedString()
        {
            // Arrange
            string plainText = "Hello World!";
            
            // Act
            string result = Cipher.Encrypt(plainText);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().NotBe(plainText);
            result.Should().NotBeEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9+/]*={0,2}$"); // Base64 pattern
        }

        [Fact]
        public void Encrypt_WithEmptyString_ShouldReturnNull()
        {
            // Arrange
            string plainText = string.Empty;
            
            // Act
            string result = Cipher.Encrypt(plainText);
            
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Encrypt_WithWhitespaceString_ShouldReturnNull()
        {
            // Arrange
            string plainText = "   ";
            
            // Act
            string result = Cipher.Encrypt(plainText);
            
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Encrypt_WithNullString_ShouldReturnNull()
        {
            // Arrange
            string plainText = null;
            
            // Act
            string result = Cipher.Encrypt(plainText);
            
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Encrypt_WithDifferentInputs_ShouldReturnDifferentResults()
        {
            // Arrange
            string input1 = "test1";
            string input2 = "test2";
            
            // Act
            string result1 = Cipher.Encrypt(input1);
            string result2 = Cipher.Encrypt(input2);
            
            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Encrypt_WithSpecialCharacters_ShouldReturnEncryptedString()
        {
            // Arrange
            string plainText = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            
            // Act
            string result = Cipher.Encrypt(plainText);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().NotBe(plainText);
            result.Should().MatchRegex("^[A-Za-z0-9+/]*={0,2}$");
        }

        [Fact]
        public void Decrypt_WithValidEncryptedString_ShouldReturnOriginalString()
        {
            // Arrange
            string originalText = "Test String for Decryption";
            string encryptedText = Cipher.Encrypt(originalText);
            
            // Act
            string result = Cipher.Decrypt(encryptedText);
            
            // Assert
            result.Should().Be(originalText);
        }

        [Fact]
        public void Decrypt_WithEmptyString_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = string.Empty;
            
            // Act
            string result = Cipher.Decrypt(encryptedText);
            
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Decrypt_WithWhitespaceString_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = "   ";
            
            // Act
            string result = Cipher.Decrypt(encryptedText);
            
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Decrypt_WithNullString_ShouldReturnNull()
        {
            // Arrange
            string encryptedText = null;
            
            // Act
            string result = Cipher.Decrypt(encryptedText);
            
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Decrypt_WithInvalidBase64String_ShouldThrowFormatException()
        {
            // Arrange
            string invalidEncryptedText = "InvalidBase64String!@#";
            
            // Act & Assert
            Action action = () => Cipher.Decrypt(invalidEncryptedText);
            action.Should().Throw<FormatException>();
        }

        [Fact]
        public void Decrypt_WithCorruptedEncryptedData_ShouldThrowCryptographicException()
        {
            // Arrange
            string corruptedData = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("corrupted data"));
            
            // Act & Assert
            Action action = () => Cipher.Decrypt(corruptedData);
            action.Should().Throw<System.Security.Cryptography.CryptographicException>();
        }

        [Fact]
        public void Hashing_WithValidString_ShouldReturnSHA256Hash()
        {
            // Arrange
            string original = "test string";
            
            // Act
            string result = Cipher.Hashing(original);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64); // SHA256 hash is 64 characters in hex
            result.Should().MatchRegex("^[0-9a-f]{64}$"); // Should be hex string
        }

        [Fact]
        public void Hashing_WithEmptyString_ShouldReturnValidHash()
        {
            // Arrange
            string original = string.Empty;
            
            // Act
            string result = Cipher.Hashing(original);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64);
            result.Should().Be("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
        }

        [Fact]
        public void Hashing_WithNullString_ShouldThrowArgumentNullException()
        {
            // Arrange
            string original = null;
            
            // Act & Assert
            Action action = () => Cipher.Hashing(original);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Hashing_WithSameInput_ShouldReturnSameHash()
        {
            // Arrange
            string original = "consistent input";
            
            // Act
            string result1 = Cipher.Hashing(original);
            string result2 = Cipher.Hashing(original);
            
            // Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public void Hashing_WithDifferentInputs_ShouldReturnDifferentResults()
        {
            // Arrange
            string input1 = "test1";
            string input2 = "test2";
            
            // Act
            string result1 = Cipher.Hashing(input1);
            string result2 = Cipher.Hashing(input2);
            
            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Hashing_WithSpecialCharacters_ShouldReturnValidHash()
        {
            // Arrange
            string original = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            
            // Act
            string result = Cipher.Hashing(original);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64);
            result.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void Hashing_WithUnicodeCharacters_ShouldReturnValidHash()
        {
            // Arrange
            string original = "Unicode test: 你好世界 🌍 🚀";
            
            // Act
            string result = Cipher.Hashing(original);
            
            // Assert
            result.Should().NotBeNull();
            result.Should().HaveLength(64);
            result.Should().MatchRegex("^[0-9a-f]{64}$");
        }

        [Fact]
        public void Hashing2Int_WithValidString_ShouldReturnInt64()
        {
            // Arrange
            string original = "test string";
            
            // Act
            long result = Cipher.Hashing2Int(original);
            
            // Assert
            result.Should().BeOfType(typeof(long));
        }

        [Fact]
        public void Hashing2Int_WithEmptyString_ShouldReturnValidInt64()
        {
            // Arrange
            string original = string.Empty;
            
            // Act
            long result = Cipher.Hashing2Int(original);
            
            // Assert
            result.Should().BeOfType(typeof(long));
        }

        [Fact]
        public void Hashing2Int_WithNullString_ShouldThrowArgumentNullException()
        {
            // Arrange
            string original = null;
            
            // Act & Assert
            Action action = () => Cipher.Hashing2Int(original);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void Hashing2Int_WithSameInput_ShouldReturnSameValue()
        {
            // Arrange
            string original = "consistent input";
            
            // Act
            long result1 = Cipher.Hashing2Int(original);
            long result2 = Cipher.Hashing2Int(original);
            
            // Assert
            result1.Should().Be(result2);
        }

        [Fact]
        public void Hashing2Int_WithDifferentInputs_ShouldReturnDifferentResults()
        {
            // Arrange
            string input1 = "test1";
            string input2 = "test2";
            
            // Act
            long result1 = Cipher.Hashing2Int(input1);
            long result2 = Cipher.Hashing2Int(input2);
            
            // Assert
            result1.Should().NotBe(result2);
        }

        [Fact]
        public void Hashing2Int_WithSpecialCharacters_ShouldReturnValidInt64()
        {
            // Arrange
            string original = "!@#$%^&*()_+-=[]{}|;':\",./<>?";
            
            // Act
            long result = Cipher.Hashing2Int(original);
            
            // Assert
            result.Should().BeOfType(typeof(long));
        }

        [Fact]
        public void EncryptDecrypt_RoundTrip_ShouldReturnOriginalString()
        {
            // Arrange
            string originalText = "This is a test string with special characters: !@#$%^&*()_+-=[]{}|;':\",./<>?";
            
            // Act
            string encrypted = Cipher.Encrypt(originalText);
            string decrypted = Cipher.Decrypt(encrypted);
            
            // Assert
            decrypted.Should().Be(originalText);
        }

        [Fact]
        public void EncryptDecrypt_WithUnicodeCharacters_ShouldReturnOriginalString()
        {
            // Arrange
            string originalText = "Unicode test: 你好世界 🌍 🚀";
            
            // Act
            string encrypted = Cipher.Encrypt(originalText);
            string decrypted = Cipher.Decrypt(encrypted);
            
            // Assert
            decrypted.Should().Be(originalText);
        }

        [Fact]
        public void EncryptDecrypt_WithLongString_ShouldReturnOriginalString()
        {
            // Arrange
            string originalText = new string('A', 1000); // 1000 character string
            
            // Act
            string encrypted = Cipher.Encrypt(originalText);
            string decrypted = Cipher.Decrypt(encrypted);
            
            // Assert
            decrypted.Should().Be(originalText);
        }

        [Fact]
        public void EncryptDecrypt_WithNumbers_ShouldReturnOriginalString()
        {
            // Arrange
            string originalText = "1234567890";
            
            // Act
            string encrypted = Cipher.Encrypt(originalText);
            string decrypted = Cipher.Decrypt(encrypted);
            
            // Assert
            decrypted.Should().Be(originalText);
        }

        [Fact]
        public void EncryptDecrypt_MultipleOperations_ShouldBeConsistent()
        {
            // Arrange
            string originalText = "Test consistency";
            
            // Act
            string encrypted1 = Cipher.Encrypt(originalText);
            string decrypted1 = Cipher.Decrypt(encrypted1);
            string encrypted2 = Cipher.Encrypt(originalText);
            string decrypted2 = Cipher.Decrypt(encrypted2);
            
            // Assert
            decrypted1.Should().Be(originalText);
            decrypted2.Should().Be(originalText);
            decrypted1.Should().Be(decrypted2);
        }

        [Fact]
        public void EncryptDecrypt_WithMixedContent_ShouldReturnOriginalString()
        {
            // Arrange
            string originalText = "Mixed content: 123 ABC !@# 你好 🌍";
            
            // Act
            string encrypted = Cipher.Encrypt(originalText);
            string decrypted = Cipher.Decrypt(encrypted);
            
            // Assert
            decrypted.Should().Be(originalText);
        }

        [Fact]
        public void Encrypt_WithLargeString_ShouldCompleteInReasonableTime()
        {
            // Arrange
            string largeText = new string('A', 10000); // 10KB string
            
            // Act & Assert
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            string result = Cipher.Encrypt(largeText);
            stopwatch.Stop();
            
            result.Should().NotBeNull();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(1000); // Should complete within 1 second
        }

        [Fact]
        public void Hashing_WithLargeString_ShouldCompleteInReasonableTime()
        {
            // Arrange
            string largeText = new string('A', 10000); // 10KB string
            
            // Act & Assert
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            string result = Cipher.Hashing(largeText);
            stopwatch.Stop();
            
            result.Should().NotBeNull();
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(100); // Should complete within 100ms
        }
    }
}

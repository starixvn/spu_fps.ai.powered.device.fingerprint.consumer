using Xunit;
using FluentAssertions;
using Moq;
using Device.FingerPrint.Core.Services;

namespace Device.FingerPrint.Core.Test
{
    public class CryptoTests
    {
        [Fact]
        public void Encrypt_WithValidText_ShouldReturnEncryptedString()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockAppSettings.Setup(x => x.CryptoPassword).Returns("testpassword");
            
            var crypto = new Crypto(mockAppSettings.Object);
            var plainText = "test123";

            // Act
            var result = crypto.Encrypt(plainText);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().NotBe(plainText);
        }

        [Fact]
        public void Encrypt_WithEmptyText_ShouldReturnEmptyString()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockAppSettings.Setup(x => x.CryptoPassword).Returns("testpassword");
            
            var crypto = new Crypto(mockAppSettings.Object);
            var plainText = "";

            // Act
            var result = crypto.Encrypt(plainText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Encrypt_WithNullText_ShouldReturnEmptyString()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockAppSettings.Setup(x => x.CryptoPassword).Returns("testpassword");
            
            var crypto = new Crypto(mockAppSettings.Object);
            string plainText = null;

            // Act
            var result = crypto.Encrypt(plainText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_WithValidEncryptedText_ShouldReturnDecryptedString()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockAppSettings.Setup(x => x.CryptoPassword).Returns("testpassword");
            
            var crypto = new Crypto(mockAppSettings.Object);
            var plainText = "test123";
            var encryptedText = crypto.Encrypt(plainText);

            // Act
            var result = crypto.Decrypt(encryptedText);

            // Assert
            result.Should().Be(plainText);
        }

        [Fact]
        public void Decrypt_WithEmptyText_ShouldReturnEmptyString()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockAppSettings.Setup(x => x.CryptoPassword).Returns("testpassword");
            
            var crypto = new Crypto(mockAppSettings.Object);
            var encryptedText = "";

            // Act
            var result = crypto.Decrypt(encryptedText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Decrypt_WithNullText_ShouldReturnEmptyString()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockAppSettings.Setup(x => x.CryptoPassword).Returns("testpassword");
            
            var crypto = new Crypto(mockAppSettings.Object);
            string encryptedText = null;

            // Act
            var result = crypto.Decrypt(encryptedText);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetMD5Hash_WithValidString_ShouldReturnMD5Hash()
        {
            // Arrange
            var mockAppSettings = new Mock<IAppSettings>();
            mockAppSettings.Setup(x => x.CryptoIV).Returns("c3eef06037e837f4ee390bda69c05023");
            mockAppSettings.Setup(x => x.CryptoSalt).Returns("234292a9fce031f2");
            mockAppSettings.Setup(x => x.CryptoPassword).Returns("testpassword");
            
            var crypto = new Crypto(mockAppSettings.Object);
            var input = "test123";

            // Act
            var result = crypto.GetMD5Hash(input);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveLength(32); // MD5 hash is 32 characters
        }
    }
}

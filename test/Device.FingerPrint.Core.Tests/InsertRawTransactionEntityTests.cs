using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Model;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class InsertRawTransactionEntityTests
    {
        [Fact]
        public void InsertRawTransactionEntity_WebRTCIPCode_ShouldBeSettableAndGettable()
        {
            // Arrange
            var entity = new InsertRawTransactionEntity();
            var webRTCIPCode = "abc123def456";

            // Act
            entity.WebRTCIPCode = webRTCIPCode;

            // Assert
            entity.WebRTCIPCode.Should().Be(webRTCIPCode);
        }

        [Fact]
        public void InsertRawTransactionEntity_WebRTCIPCode_ShouldAcceptNull()
        {
            // Arrange
            var entity = new InsertRawTransactionEntity();

            // Act
            entity.WebRTCIPCode = null;

            // Assert
            entity.WebRTCIPCode.Should().BeNull();
        }

        [Fact]
        public void InsertRawTransactionEntity_WebRTCIPCode_ShouldHaveDefaultNullValue()
        {
            // Arrange & Act
            var entity = new InsertRawTransactionEntity();

            // Assert
            entity.WebRTCIPCode.Should().BeNull();
        }

        [Fact]
        public void InsertRawTransactionEntity_WebRTCIPCode_ShouldAcceptEmptyString()
        {
            // Arrange
            var entity = new InsertRawTransactionEntity();

            // Act
            entity.WebRTCIPCode = string.Empty;

            // Assert
            entity.WebRTCIPCode.Should().Be(string.Empty);
        }

        [Theory]
        [InlineData("098f6bcd4621d373cade4e832627b4f6")] // MD5 of "test"
        [InlineData("5d41402abc4b2a76b9719d911017c592")] // MD5 of "hello"
        [InlineData("202cb962ac59075b964b07152d234b70")] // MD5 of "123"
        public void InsertRawTransactionEntity_WebRTCIPCode_WithValidMD5Hash_ShouldWork(string md5Hash)
        {
            // Arrange
            var entity = new InsertRawTransactionEntity();

            // Act
            entity.WebRTCIPCode = md5Hash;

            // Assert
            entity.WebRTCIPCode.Should().Be(md5Hash);
            entity.WebRTCIPCode.Should().HaveLength(32);
            entity.WebRTCIPCode.Should().MatchRegex("^[a-f0-9]{32}$"); // Should be lowercase hex
        }

        [Fact]
        public void InsertRawTransactionEntity_JsonSerialization_WithWebRTCIPCode_ShouldWork()
        {
            // Arrange
            var entity = new InsertRawTransactionEntity
            {
                LoginName = "testUser",
                Action = "login",
                WebRTCIPCode = "098f6bcd4621d373cade4e832627b4f6",
                TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var json = JsonConvert.SerializeObject(entity);
            var deserializedEntity = JsonConvert.DeserializeObject<InsertRawTransactionEntity>(json);

            // Assert
            deserializedEntity.Should().NotBeNull();
            deserializedEntity!.LoginName.Should().Be("testUser");
            deserializedEntity.Action.Should().Be("login");
            deserializedEntity.WebRTCIPCode.Should().Be("098f6bcd4621d373cade4e832627b4f6");
        }

        [Fact]
        public void InsertRawTransactionEntity_JsonSerialization_WithNullWebRTCIPCode_ShouldWork()
        {
            // Arrange
            var entity = new InsertRawTransactionEntity
            {
                LoginName = "testUser",
                Action = "login",
                WebRTCIPCode = null,
                TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var json = JsonConvert.SerializeObject(entity);
            var deserializedEntity = JsonConvert.DeserializeObject<InsertRawTransactionEntity>(json);

            // Assert
            deserializedEntity.Should().NotBeNull();
            deserializedEntity!.LoginName.Should().Be("testUser");
            deserializedEntity.Action.Should().Be("login");
            deserializedEntity.WebRTCIPCode.Should().BeNull();
        }

        [Fact]
        public void InsertRawTransactionEntity_WebRTCIPCode_WithActualWebRTCIPModel_ShouldMatchCode()
        {
            // Arrange
            var webRTCIP = new WebRTCIPModel("192.168.1.100");
            var entity = new InsertRawTransactionEntity();

            // Act
            entity.WebRTCIPCode = webRTCIP.Code;

            // Assert
            entity.WebRTCIPCode.Should().Be(webRTCIP.Code);
            entity.WebRTCIPCode.Should().NotBeNullOrEmpty();
            entity.WebRTCIPCode.Should().HaveLength(32);
        }

        [Fact]
        public void InsertRawTransactionEntity_MultipleInstances_WithSameWebRTCIPCode_ShouldBeEqual()
        {
            // Arrange
            var code = "098f6bcd4621d373cade4e832627b4f6";
            var entity1 = new InsertRawTransactionEntity { WebRTCIPCode = code };
            var entity2 = new InsertRawTransactionEntity { WebRTCIPCode = code };

            // Act & Assert
            entity1.WebRTCIPCode.Should().Be(entity2.WebRTCIPCode);
        }

        [Fact]
        public void InsertRawTransactionEntity_MultipleInstances_WithDifferentWebRTCIPCode_ShouldNotBeEqual()
        {
            // Arrange
            var entity1 = new InsertRawTransactionEntity { WebRTCIPCode = "098f6bcd4621d373cade4e832627b4f6" };
            var entity2 = new InsertRawTransactionEntity { WebRTCIPCode = "5d41402abc4b2a76b9719d911017c592" };

            // Act & Assert
            entity1.WebRTCIPCode.Should().NotBe(entity2.WebRTCIPCode);
        }

        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.1")]
        [InlineData("172.16.0.1")]
        [InlineData("fe80::1")]
        public void InsertRawTransactionEntity_WebRTCIPCode_FromVariousIPFormats_ShouldBeConsistent(string ipAddress)
        {
            // Arrange
            var webRTCIP1 = new WebRTCIPModel(ipAddress);
            var webRTCIP2 = new WebRTCIPModel(ipAddress);
            var entity1 = new InsertRawTransactionEntity { WebRTCIPCode = webRTCIP1.Code };
            var entity2 = new InsertRawTransactionEntity { WebRTCIPCode = webRTCIP2.Code };

            // Act & Assert
            entity1.WebRTCIPCode.Should().Be(entity2.WebRTCIPCode);
            entity1.WebRTCIPCode.Should().HaveLength(32);
        }

        [Fact]
        public void InsertRawTransactionEntity_WebRTCIPCode_ShouldSupportLongStrings()
        {
            // Arrange
            var entity = new InsertRawTransactionEntity();
            var longCode = new string('a', 100); // Very long string

            // Act
            entity.WebRTCIPCode = longCode;

            // Assert
            entity.WebRTCIPCode.Should().Be(longCode);
            entity.WebRTCIPCode.Should().HaveLength(100);
        }
    }
}

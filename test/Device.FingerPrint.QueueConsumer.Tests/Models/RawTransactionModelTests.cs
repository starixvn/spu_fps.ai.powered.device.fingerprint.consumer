using Device.FingerPrint.Core;
using Device.FingerPrint.Model;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    public class RawTransactionModelTests
    {
        [Fact]
        public void RawTransactionModel_WebRTCIP_ShouldBeSettableAndGettable()
        {
            // Arrange
            var rawTransaction = new RawTransactionModel();
            var webRTCIP = new WebRTCIPModel("192.168.1.100");

            // Act
            rawTransaction.WebRTCIP = webRTCIP;

            // Assert
            rawTransaction.WebRTCIP.Should().NotBeNull();
            rawTransaction.WebRTCIP.Should().Be(webRTCIP);
            rawTransaction.WebRTCIP!.Details.Should().Be("192.168.1.100");
            rawTransaction.WebRTCIP.Code.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void RawTransactionModel_WebRTCIP_ShouldAcceptNull()
        {
            // Arrange
            var rawTransaction = new RawTransactionModel();

            // Act
            rawTransaction.WebRTCIP = null;

            // Assert
            rawTransaction.WebRTCIP.Should().BeNull();
        }

        [Fact]
        public void RawTransactionModel_WebRTCIP_ShouldHaveDefaultNullValue()
        {
            // Arrange & Act
            var rawTransaction = new RawTransactionModel();

            // Assert
            rawTransaction.WebRTCIP.Should().BeNull();
        }

        [Fact]
        public void RawTransactionModel_WebRTCIP_WithEmptyDetails_ShouldWork()
        {
            // Arrange
            var rawTransaction = new RawTransactionModel();
            var webRTCIP = new WebRTCIPModel(string.Empty);

            // Act
            rawTransaction.WebRTCIP = webRTCIP;

            // Assert
            rawTransaction.WebRTCIP.Should().NotBeNull();
            rawTransaction.WebRTCIP!.Details.Should().Be(string.Empty);
            rawTransaction.WebRTCIP.Code.Should().Be(string.Empty);
        }

        [Fact]
        public void RawTransactionModel_JsonSerialization_WithWebRTCIP_ShouldWork()
        {
            // Arrange
            var rawTransaction = new RawTransactionModel
            {
                LoginName = "testUser",
                IP = "10.0.0.1",
                WebRTCIP = new WebRTCIPModel("192.168.1.100"),
                TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var json = JsonConvert.SerializeObject(rawTransaction, JsonSerializerSettingsFactory.Instance);
            var deserializedTransaction = JsonConvert.DeserializeObject<RawTransactionModel>(json, JsonSerializerSettingsFactory.Instance);

            // Assert
            deserializedTransaction.Should().NotBeNull();
            deserializedTransaction!.LoginName.Should().Be("testUser");
            deserializedTransaction.IP.Should().Be("10.0.0.1");
            deserializedTransaction.WebRTCIP.Should().NotBeNull();
            deserializedTransaction.WebRTCIP!.Details.Should().Be("192.168.1.100");
            deserializedTransaction.WebRTCIP.Code.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void RawTransactionModel_JsonSerialization_WithNullWebRTCIP_ShouldWork()
        {
            // Arrange
            var rawTransaction = new RawTransactionModel
            {
                LoginName = "testUser",
                IP = "10.0.0.1",
                WebRTCIP = null,
                TransTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc),
                CreatedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc)
            };

            // Act
            var json = JsonConvert.SerializeObject(rawTransaction, JsonSerializerSettingsFactory.Instance);
            var deserializedTransaction = JsonConvert.DeserializeObject<RawTransactionModel>(json, JsonSerializerSettingsFactory.Instance);

            // Assert
            deserializedTransaction.Should().NotBeNull();
            deserializedTransaction!.LoginName.Should().Be("testUser");
            deserializedTransaction.IP.Should().Be("10.0.0.1");
            deserializedTransaction.WebRTCIP.Should().BeNull();
        }

        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.1")]
        [InlineData("172.16.0.1")]
        [InlineData("fe80::1")]
        [InlineData("")]
        public void RawTransactionModel_WebRTCIP_WithVariousIPFormats_ShouldWork(string ipAddress)
        {
            // Arrange
            var rawTransaction = new RawTransactionModel();
            var webRTCIP = new WebRTCIPModel(ipAddress);

            // Act
            rawTransaction.WebRTCIP = webRTCIP;

            // Assert
            rawTransaction.WebRTCIP.Should().NotBeNull();
            rawTransaction.WebRTCIP!.Details.Should().Be(ipAddress);
            
            if (string.IsNullOrEmpty(ipAddress))
            {
                rawTransaction.WebRTCIP.Code.Should().Be(string.Empty);
            }
            else
            {
                rawTransaction.WebRTCIP.Code.Should().NotBeNullOrEmpty();
                rawTransaction.WebRTCIP.Code.Should().HaveLength(32); // MD5 hash length
            }
        }

        [Fact]
        public void RawTransactionModel_MultipleInstances_WithSameWebRTCIPDetails_ShouldHaveSameCode()
        {
            // Arrange
            var transaction1 = new RawTransactionModel { WebRTCIP = new WebRTCIPModel("192.168.1.100") };
            var transaction2 = new RawTransactionModel { WebRTCIP = new WebRTCIPModel("192.168.1.100") };

            // Act & Assert
            transaction1.WebRTCIP!.Code.Should().Be(transaction2.WebRTCIP!.Code);
            transaction1.WebRTCIP.Details.Should().Be(transaction2.WebRTCIP.Details);
        }

        [Fact]
        public void RawTransactionModel_MultipleInstances_WithDifferentWebRTCIPDetails_ShouldHaveDifferentCode()
        {
            // Arrange
            var transaction1 = new RawTransactionModel { WebRTCIP = new WebRTCIPModel("192.168.1.100") };
            var transaction2 = new RawTransactionModel { WebRTCIP = new WebRTCIPModel("192.168.1.101") };

            // Act & Assert
            transaction1.WebRTCIP!.Code.Should().NotBe(transaction2.WebRTCIP!.Code);
            transaction1.WebRTCIP.Details.Should().NotBe(transaction2.WebRTCIP.Details);
        }
    }
}

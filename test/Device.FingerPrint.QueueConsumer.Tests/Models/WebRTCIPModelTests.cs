using Device.FingerPrint.Model;
using FluentAssertions;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    public class WebRTCIPModelTests
    {
        [Fact]
        public void WebRTCIPModel_Constructor_WithValidDetails_ShouldSetPropertiesCorrectly()
        {
            // Arrange
            var details = "192.168.1.100";

            // Act
            var webRTCIP = new WebRTCIPModel(details);

            // Assert
            webRTCIP.Details.Should().Be(details);
            webRTCIP.Code.Should().NotBeNullOrEmpty();
            webRTCIP.Code.Should().HaveLength(32); // MD5 hash is 32 characters
        }

        [Fact]
        public void WebRTCIPModel_Constructor_WithEmptyDetails_ShouldSetEmptyDetails()
        {
            // Arrange
            var details = string.Empty;

            // Act
            var webRTCIP = new WebRTCIPModel(details);

            // Assert
            webRTCIP.Details.Should().Be(string.Empty);
            webRTCIP.Code.Should().Be(string.Empty);
        }

        [Fact]
        public void WebRTCIPModel_Constructor_WithNullDetails_ShouldSetEmptyDetails()
        {
            // Arrange
            string? details = null;

            // Act
            var webRTCIP = new WebRTCIPModel(details);

            // Assert
            webRTCIP.Details.Should().Be(string.Empty);
            webRTCIP.Code.Should().Be(string.Empty);
        }

        [Fact]
        public void WebRTCIPModel_Code_ShouldBeConsistentForSameDetails()
        {
            // Arrange
            var details = "192.168.1.100";

            // Act
            var webRTCIP1 = new WebRTCIPModel(details);
            var webRTCIP2 = new WebRTCIPModel(details);

            // Assert
            webRTCIP1.Code.Should().Be(webRTCIP2.Code);
        }

        [Fact]
        public void WebRTCIPModel_Code_ShouldBeDifferentForDifferentDetails()
        {
            // Arrange
            var details1 = "192.168.1.100";
            var details2 = "192.168.1.101";

            // Act
            var webRTCIP1 = new WebRTCIPModel(details1);
            var webRTCIP2 = new WebRTCIPModel(details2);

            // Assert
            webRTCIP1.Code.Should().NotBe(webRTCIP2.Code);
        }

        [Fact]
        public void WebRTCIPModel_Code_ShouldBeLowerCase()
        {
            // Arrange
            var details = "ABCDEFG";

            // Act
            var webRTCIP = new WebRTCIPModel(details);

            // Assert
            webRTCIP.Code.Should().BeEquivalentTo(webRTCIP.Code.ToLowerInvariant());
        }

        [Fact]
        public void WebRTCIPModel_Code_ShouldNotContainDashes()
        {
            // Arrange
            var details = "test-ip-address";

            // Act
            var webRTCIP = new WebRTCIPModel(details);

            // Assert
            webRTCIP.Code.Should().NotContain("-");
        }

        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.1")]
        [InlineData("172.16.0.1")]
        [InlineData("fe80::1")]
        public void WebRTCIPModel_Constructor_WithVariousIPFormats_ShouldWork(string ipAddress)
        {
            // Act
            var webRTCIP = new WebRTCIPModel(ipAddress);

            // Assert
            webRTCIP.Details.Should().Be(ipAddress);
            webRTCIP.Code.Should().NotBeNullOrEmpty();
            webRTCIP.Code.Should().HaveLength(32);
        }

        [Fact]
        public void WebRTCIPModel_JsonSerialization_ShouldSerializeAsString()
        {
            // Arrange
            var details = "192.168.1.100";
            var webRTCIP = new WebRTCIPModel(details);

            // Act
            var json = JsonConvert.SerializeObject(webRTCIP);

            // Assert
            json.Should().Be($"\"{details}\"");
        }

        [Fact]
        public void WebRTCIPModel_JsonDeserialization_ShouldDeserializeFromString()
        {
            // Arrange
            var details = "192.168.1.100";
            var json = $"\"{details}\"";

            // Act
            var webRTCIP = JsonConvert.DeserializeObject<WebRTCIPModel>(json);

            // Assert
            webRTCIP.Should().NotBeNull();
            webRTCIP.Details.Should().Be(details);
            webRTCIP.Code.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void WebRTCIPModel_JsonRoundTrip_ShouldMaintainDetails()
        {
            // Arrange
            var originalDetails = "192.168.1.100";
            var originalWebRTCIP = new WebRTCIPModel(originalDetails);

            // Act
            var json = JsonConvert.SerializeObject(originalWebRTCIP);
            var deserializedWebRTCIP = JsonConvert.DeserializeObject<WebRTCIPModel>(json);

            // Assert
            deserializedWebRTCIP.Should().NotBeNull();
            deserializedWebRTCIP.Details.Should().Be(originalWebRTCIP.Details);
            deserializedWebRTCIP.Code.Should().Be(originalWebRTCIP.Code);
        }

        [Fact]
        public void WebRTCIPModel_JsonDeserialization_WithEmptyString_ShouldWork()
        {
            // Arrange
            var json = "\"\"";

            // Act
            var webRTCIP = JsonConvert.DeserializeObject<WebRTCIPModel>(json);

            // Assert
            webRTCIP.Should().NotBeNull();
            webRTCIP.Details.Should().Be(string.Empty);
            webRTCIP.Code.Should().Be(string.Empty);
        }

        [Fact]
        public void WebRTCIPModel_JsonDeserialization_WithNull_ShouldCreateEmptyModel()
        {
            // Arrange
            var json = "null";

            // Act
            var webRTCIP = JsonConvert.DeserializeObject<WebRTCIPModel>(json);

            // Assert
            // Note: Due to the custom JsonConverter, deserializing null creates an empty model
            webRTCIP.Should().NotBeNull();
            webRTCIP!.Details.Should().Be(string.Empty);
            webRTCIP.Code.Should().Be(string.Empty);
        }

        [Fact]
        public void WebRTCIPModel_Properties_ShouldBeReadOnly()
        {
            // Arrange & Act
            // Properties should only have getters, not setters
            var detailsProperty = typeof(WebRTCIPModel).GetProperty(nameof(WebRTCIPModel.Details));
            var codeProperty = typeof(WebRTCIPModel).GetProperty(nameof(WebRTCIPModel.Code));

            // Assert
            detailsProperty.Should().NotBeNull();
            detailsProperty!.SetMethod.Should().BeNull(); // No setter

            codeProperty.Should().NotBeNull();
            codeProperty!.SetMethod.Should().BeNull(); // No setter
        }

        [Fact]
        public void WebRTCIPModel_KnownValue_ShouldGenerateExpectedHash()
        {
            // Arrange
            var details = "test";
            var expectedCode = "098f6bcd4621d373cade4e832627b4f6"; // MD5 of "test"

            // Act
            var webRTCIP = new WebRTCIPModel(details);

            // Assert
            webRTCIP.Code.Should().Be(expectedCode);
        }
    }
}

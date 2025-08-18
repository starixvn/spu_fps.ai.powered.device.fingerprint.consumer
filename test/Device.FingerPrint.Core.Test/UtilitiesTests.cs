using Xunit;
using FluentAssertions;
using Device.FingerPrint.Core;
using System.Collections.Generic;
using System.Linq;

namespace Device.FingerPrint.Core.Test
{
    public class UtilitiesTests
    {
        [Fact]
        public void GetURLFromDeviceCode_WithValidDeviceCode_ShouldReturnURL()
        {
            // Arrange
            var deviceCode = "part1###part2###part3###part4###part5###part6###part7###https://example.com###part9";

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert
            result.Should().Be("https://example.com");
        }

        [Fact]
        public void GetURLFromDeviceCode_WithEmptyDeviceCode_ShouldReturnEmptyString()
        {
            // Arrange
            var deviceCode = "";

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetURLFromDeviceCode_WithNullDeviceCode_ShouldReturnEmptyString()
        {
            // Arrange
            string deviceCode = null;

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetURLFromDeviceCode_WithLessThan8Parts_ShouldReturnEmptyString()
        {
            // Arrange
            var deviceCode = "part1###part2###part3###part4###part5###part6###part7";

            // Act
            var result = Utilities.GetURLFromDeviceCode(deviceCode);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetUserAgentFromDeviceCode_WithValidDeviceCode_ShouldReturnUserAgent()
        {
            // Arrange
            var deviceCode = "part1###part2###part3###Mozilla/5.0###part5###part6###part7###part8";

            // Act
            var result = Utilities.GetUserAgentFromDeviceCode(deviceCode);

            // Assert
            result.Should().Be("Mozilla/5.0");
        }

        [Fact]
        public void GetUserAgentFromDeviceCode_WithLessThan4Parts_ShouldReturnEmptyString()
        {
            // Arrange
            var deviceCode = "part1###part2###part3";

            // Act
            var result = Utilities.GetUserAgentFromDeviceCode(deviceCode);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void IsGuidDeviceId_WithValidGuid_ShouldReturnTrue()
        {
            // Arrange
            var deviceCode = "12345678-1234-1234-1234-123456789012";

            // Act
            var result = Utilities.IsGuidDeviceId(deviceCode);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsGuidDeviceId_WithInvalidGuid_ShouldReturnFalse()
        {
            // Arrange
            var deviceCode = "invalid-guid";

            // Act
            var result = Utilities.IsGuidDeviceId(deviceCode);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetDeviceCodeFromCapturedData_WithValidData_ShouldReturnDeviceCode()
        {
            // Arrange
            var capturedData = "part1###part2###part3###part4###part5###part6###device123###part8";

            // Act
            var result = Utilities.GetDeviceCodeFromCapturedData(capturedData);

            // Assert
            result.Should().Be("device123");
        }

        [Fact]
        public void GetDeviceCodeFromCapturedData_WithLessThan7Parts_ShouldReturnEmptyString()
        {
            // Arrange
            var capturedData = "part1###part2###part3###part4###part5###part6";

            // Act
            var result = Utilities.GetDeviceCodeFromCapturedData(capturedData);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetAnalysis_WithValidData_ShouldReturnAnalysis()
        {
            // Arrange
            var capturedData = "part1###part2###part3###part4###part5###part6###part7###part8###analysis123";

            // Act
            var result = Utilities.GetAnalysis(capturedData);

            // Assert
            result.Should().Be("analysis123");
        }

        [Fact]
        public void GetAnalysis_WithLessThan9Parts_ShouldReturnEmptyString()
        {
            // Arrange
            var capturedData = "part1###part2###part3###part4###part5###part6###part7###part8";

            // Act
            var result = Utilities.GetAnalysis(capturedData);

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public void Chunk_WithValidData_ShouldReturnChunks()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var chunkSize = 3;

            // Act
            var result = data.Chunk(chunkSize).ToList();

            // Assert
            result.Should().HaveCount(4);
            result[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            result[1].Should().BeEquivalentTo(new[] { 4, 5, 6 });
            result[2].Should().BeEquivalentTo(new[] { 7, 8, 9 });
            result[3].Should().BeEquivalentTo(new[] { 10 });
        }

        [Fact]
        public void Split_WithValidData_ShouldReturnSplits()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var parts = 3;

            // Act
            var result = data.Split(parts).ToList();

            // Assert
            result.Should().HaveCount(4);
        }

        [Fact]
        public void Partition_WithValidData_ShouldReturnPartitions()
        {
            // Arrange
            var data = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var size = 3;

            // Act
            var result = data.Partition(size).ToList();

            // Assert
            result.Should().HaveCount(4);
            result[0].Should().BeEquivalentTo(new[] { 1, 2, 3 });
            result[1].Should().BeEquivalentTo(new[] { 4, 5, 6 });
            result[2].Should().BeEquivalentTo(new[] { 7, 8, 9 });
            result[3].Should().BeEquivalentTo(new[] { 10 });
        }
    }
}

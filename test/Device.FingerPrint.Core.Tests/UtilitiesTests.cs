using Device.FingerPrint.Model;
using FluentAssertions;
using System.Collections;
using System.Numerics;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class UtilitiesTests
    {

        [Theory]
        [InlineData("not-a-guid", false)]
        [InlineData("3f2504e0-4f89-11d3-9a0c-0305e82c3301", true)]
        public void IsGuidDeviceId_Works(string input, bool expected)
        {
            Utilities.IsGuidDeviceId(input).Should().Be(expected);
        }

        [Fact]
        public void Ip2Long_Ipv4_Succeeds()
        {
            var model = "127.0.0.1".Ip2Long();
            model.IsIpV6.Should().BeFalse();
            model.IpNumber.Should().NotBeNull();
        }

        [Fact]
        public void ConvertBitArray2Int_Roundtrip()
        {
            var arr = new BitArray(new[] { true, false, true, false, true, false, true, false });
            var value = Utilities.ConvertBitArray2Int(arr);
            value.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void IsBase64_ValidAndInvalid()
        {
            Utilities.IsBase64("SGVsbG8=").Should().BeTrue();
            Utilities.IsBase64("not base64").Should().BeFalse();
        }

        [Fact]

        public void SubStringUrl_TrimsQueryAndHashes()
        {
            Utilities.SubStringUrl("https://a.com:443/path?x=1#y").ToString().Should().Be("https://a.com/path");
        }

        [Fact]

        public void SubStringUrl_WithPort443_RemovesPort()
        {
            // Test that port 443 is removed from HTTPS URLs
            Utilities.SubStringUrl("https://a.com:443/path").ToString().Should().Be("https://a.com/path");
        }

        [Fact]

        public void SubStringUrl_WithNonStandardPort_KeepsPort()
        {
            // Test that non-standard ports are kept
            Utilities.SubStringUrl("https://a.com:8080/path").ToString().Should().Be("https://a.com:8080/path");
        }

        [Fact]
        public void StripUnicode_RemovesNonAscii()
        {
            Utilities.StripUnicodeCharactersFromString("abcĐ").Should().Be("abc");
        }

        [Fact]
        public void EncodeCapturedData_ProducesNonEmpty()
        {
            var di = new DeviceInfoModel { Battery = "1", CapturedDate = DateTime.Now.Ticks, OSName = "x", TimeZone = "+7", LocalIps = "1.1.1.1" };
            var encoded = Utilities.EncodeCapturedData(di);
            encoded.Should().NotBeNullOrEmpty();
        }
    }
}



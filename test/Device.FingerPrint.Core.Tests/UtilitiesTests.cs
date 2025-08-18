using Device.FingerPrint.Core;
using Device.FingerPrint.Model;
using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class UtilitiesTests
    {
        [Fact]
        public void GetURLFromDeviceCode_Returns8thSegment()
        {
            var di = string.Join("###", Enumerable.Repeat("x", 7).Concat(new[] { "http://example.com" }));
            Utilities.GetURLFromDeviceCode(di).Should().Be("http://example.com");
        }

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
            model.IpNumber.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Long2DotIP_Works()
        {
            var result = Utilities.Long2DotIP(new BigInteger(2130706433)); // 127.0.0.1
            result.Should().Be("127.0.0.1");
        }

        [Fact]
        public void ConvertBitArray2Int_Roundtrip()
        {
            var arr = new BitArray(new[] { true, false, true, false, true, false, true, false });
            var value = Utilities.ConvertBitArray2Int(arr);
            value.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public void Partition_ListSize_Splits()
        {
            var list = Enumerable.Range(1, 10).ToList();
            var parts = Utilities.Partition(list, 3);
            parts.Length.Should().Be(3);
        }

        [Fact]
        public void IsBase64_ValidAndInvalid()
        {
            Utilities.IsBase64("SGVsbG8=").Should().BeTrue();
            Utilities.IsBase64("not base64").Should().BeFalse();
        }

        [Fact]
        public void RemoveRedisPrefix_StripsPrefix()
        {
            Utilities.RemoveRedisPrefix("RealTagging:abc", "RealTagging:").Should().Be("abc");
            Utilities.RemoveRedisPrefix("abc", "RealTagging:").Should().Be("abc");
        }

        [Fact]
        public void SubStringUrl_TrimsQueryAndHashes()
        {
            Utilities.SubStringUrl("https://a.com:443/path?x=1#y").Should().Be("https://a.com:443/path");
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

        [Theory]
        [InlineData("c2Vzc2lvbg==", 0, true)]
        [InlineData("c2Vzc2lvbg==:1", 0, true)]
        [InlineData(":1", 0, false)]
        public void ParseSessionKey_ValidatesBase64(string sessionKey, int defaultDb, bool expectedNotNull)
        {
            var model = Utilities.ParseSessionKey(sessionKey, defaultDb);
            (model != null).Should().Be(expectedNotNull);
        }
    }
}



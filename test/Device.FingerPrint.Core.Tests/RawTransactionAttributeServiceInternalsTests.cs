using Device.FingerPrint.Model;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
	public class RawTransactionAttributeServiceInternalsTests
	{
		private static T InvokeStatic<T>(string methodName, params object[] args)
		{
			var mi = typeof(RawTransactionAttributeService).GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static);
			mi.Should().NotBeNull();
			var result = mi!.Invoke(null, args);
			return result is null ? default! : (T)result;
		}

		[Fact]
		public void CompareVersionsWithTryParse_CoversBranches()
		{
			InvokeStatic<int>("CompareVersionsWithTryParse", "1.2.3", "2.0.0").Should().Be(0);
			InvokeStatic<int>("CompareVersionsWithTryParse", "1.2.3", "1.2.0").Should().Be(1);
			InvokeStatic<int>("CompareVersionsWithTryParse", "invalid", "1.0.0").Should().Be(-1);
		}

		[Fact]
		public void DeserializeToDictionary_NullAndValidJson()
		{
			var empty = InvokeStatic<Dictionary<string, string>>("DeserializeToDictionary", new object[] { null! });
			empty.Should().NotBeNull();
			empty.Count.Should().Be(0);

			var json = "{\"isIncognitoMode\": true, \"number\": 123, \"obj\": {\"a\":1}}";
			var dict = InvokeStatic<Dictionary<string, string>>("DeserializeToDictionary", json);
			dict.Should().ContainKey("isIncognitoMode");
			dict.Should().ContainKey("number");
			dict.Should().ContainKey("obj");
		}

		[Fact]
		public void UpdateIsIncognitoMode_SetsAndIgnores()
		{
			var model = new RawTransactionModel();
			var dict = new Dictionary<string, string> { { "isIncognitoMode", "true" } };

			var mi = typeof(RawTransactionAttributeService).GetMethod("UpdateIsIncognitoMode", BindingFlags.NonPublic | BindingFlags.Static);
			mi.Should().NotBeNull();
			mi!.Invoke(null, new object[] { model, dict, "Chrome" });
			model.IsIncognitoMode.Should().BeTrue();

			// Trường hợp bị bỏ qua theo browser
			model.IsIncognitoMode = null;
			mi.Invoke(null, new object[] { model, dict, "UC Browser" });
			model.IsIncognitoMode.Should().BeFalse();
		}

		[Fact]
		public void CleanUserAgentFuzzy_RemovesTrackingTokens()
		{
			var ua = "Mozilla/5.0 UUID/abc HTTP_BB_FORWARDED/xyz [Tracking] SessionId/123";
			var cleaned = InvokeStatic<string>("CleanUserAgentFuzzy", ua);
			cleaned.Should().Be("Mozilla/5.0");
		}

		[Fact]
		public void IsHexString_And_CheckGuid_Simple()
		{
			InvokeStatic<bool>("IsHexString", "abcdef123").Should().BeTrue();
			InvokeStatic<bool>("IsHexString", "zzzzz").Should().BeFalse();

			// CheckGuid: chuỗi 32 ký tự hex hợp lệ ở format N => true
			InvokeStatic<bool>("CheckGuid", new string('a', 32)).Should().BeTrue();
			// Độ dài khác 32 => false
			InvokeStatic<bool>("CheckGuid", "123").Should().BeFalse();
		}

		[Fact]
		public void GetTaggingValue_ReturnsEmptyForInvalid()
		{
			// Không phải hex => rỗng
			InvokeStatic<string>("GetTaggingValue", "not-hex", "not-hex").Should().BeEmpty();
			// Hex nhưng decrypt sẽ thất bại trong try/catch => vẫn rỗng
			InvokeStatic<string>("GetTaggingValue", "abcdef", "abcdef").Should().BeEmpty();
		}
	}
}

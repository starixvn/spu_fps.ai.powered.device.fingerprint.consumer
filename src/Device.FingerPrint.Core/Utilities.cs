using Device.FingerPrint.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Device.FingerPrint.Core
{
    public static class Utilities
    {
        private const int MAX_ANSI_CODE = 255;
        private const int IPV6_BYTE_LENGTH = 16;
        private const int BASE64_MODULO = 4;
        private const int BIT_ARRAY_PADDING_LENGTH = 8;
        private const int UNICODE_REGEX_TIMEOUT_SECONDS = 1;
        private const int MIN_SEGMENTS_FOR_CASINO_COLLAPSE = 4;

        private const string NOT_AVAILABLE = "N/A";
        private const string CASINO_PATH = "casino";
        private const string COLLAPSED_CASINO_PATH_FORMAT = "/casino/.../casino/{0}";

        private static readonly Regex UnicodeRegex = new Regex(
            @"[^\u0000-\u007F]",
            RegexOptions.Compiled,
            TimeSpan.FromSeconds(UNICODE_REGEX_TIMEOUT_SECONDS));

        public static bool IsGuidDeviceId(string deviceCode)
        {
            return Guid.TryParse(deviceCode, out _);
        }

        public static IpModel Ip2Long(this string ipAddress)
        {
            if (!IPAddress.TryParse(ipAddress, out IPAddress ipAddress2))
            {
                return CreateInvalidIpModel();
            }

            return ConvertToIpModel(ipAddress2);
        }

        public static int ConvertBitArray2Int(BitArray bitArray)
        {
            if (bitArray == null)
            {
                throw new ArgumentNullException(nameof(bitArray));
            }

            var binString = ToBitString(bitArray);
            return Convert.ToInt32(binString, 2);
        }

        public static int[] ConvertInt2BitArray(int input)
        {
            string binaryString = Convert.ToString(input, 2);
            return binaryString.PadLeft(BIT_ARRAY_PADDING_LENGTH, '0')
                              .Select(c => int.Parse(c.ToString()))
                              .ToArray();
        }

        public static string ToBitString(BitArray bits)
        {
            if (bits == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(bits.Count);
            for (var i = 0; i < bits.Count; i++)
            {
                sb.Append(bits[i] ? '1' : '0');
            }

            return sb.ToString();
        }

        public static bool ContainsUnicodeCharacter(string input)
        {
            return !string.IsNullOrEmpty(input) && input.Any(c => c > MAX_ANSI_CODE);
        }

        public static string? ConvertNA2Empty(string? data)
        {
            return data == NOT_AVAILABLE ? null : data;
        }

        public static Uri? SubStringUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            var cleanUrl = RemoveQueryAndFragment(url);
            return CollapseCasinoPaths(cleanUrl);
        }

        public static string StripUnicodeCharactersFromString(string? inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return string.Empty;
            }

            try
            {
                return UnicodeRegex.Replace(inputValue, string.Empty);
            }
            catch (RegexMatchTimeoutException)
            {
                return inputValue; // Return original if timeout occurs
            }
        }

        public static bool IsBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String) || base64String.Length % BASE64_MODULO != 0)
            {
                return false;
            }

            try
            {
                Convert.FromBase64String(base64String);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static string RemoveRedisPrefix(string input, string prefix)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            if (string.IsNullOrEmpty(prefix))
            {
                return input;
            }

            return input.StartsWith(prefix, StringComparison.Ordinal)
                ? input.Substring(prefix.Length)
                : input;
        }

        private static IpModel CreateInvalidIpModel()
        {
            return new IpModel
            {
                IpNumber = -1,
                IsIpV6 = false
            };
        }

        private static IpModel ConvertToIpModel(IPAddress ipAddress)
        {
            byte[] addressBytes = ipAddress.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                addressBytes = ReverseByteArray(addressBytes);
            }

            return addressBytes.Length > IPV6_BYTE_LENGTH / 2
                ? CreateIPv6Model(addressBytes)
                : CreateIPv4Model(addressBytes);
        }

        private static byte[] ReverseByteArray(byte[] originalBytes)
        {
            var byteList = new List<byte>(originalBytes);
            byteList.Reverse();
            return byteList.ToArray();
        }

        private static IpModel CreateIPv6Model(byte[] addressBytes)
        {
            BigInteger ipNumber = BitConverter.ToUInt64(addressBytes, 8);
            ipNumber <<= 64;
            ipNumber += BitConverter.ToUInt64(addressBytes, 0);

            return new IpModel
            {
                IpNumber = ipNumber,
                IsIpV6 = true
            };
        }

        private static IpModel CreateIPv4Model(byte[] addressBytes)
        {
            BigInteger ipNumber = BitConverter.ToUInt32(addressBytes, 0);

            return new IpModel
            {
                IpNumber = ipNumber,
                IsIpV6 = false
            };
        }

        private static string RemoveQueryAndFragment(string url)
        {
            int cutIndex = url.IndexOfAny(new[] { '?', '#' });
            return cutIndex != -1 ? url.Substring(0, cutIndex) : url;
        }

        private static Uri? CollapseCasinoPaths(string rawUrl)
        {
            if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out Uri uri))
            {
                return null;
            }

            var segments = GetPathSegments(uri);

            if (!ShouldCollapseCasinoPaths(segments))
            {
                return uri;
            }

            return CreateCollapsedCasinoUri(uri, segments);
        }

        private static string[] GetPathSegments(Uri uri)
        {
            return uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static bool ShouldCollapseCasinoPaths(string[] segments)
        {
            if (segments.Length < MIN_SEGMENTS_FOR_CASINO_COLLAPSE)
            {
                return false;
            }

            var firstSegment = segments[0];
            var lastSegment = segments[^1];

            // Check if first is casino and last is not casino
            if (!string.Equals(firstSegment, CASINO_PATH, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(lastSegment, CASINO_PATH, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            // Check if middle segments are all "casino"
            for (var i = 1; i < segments.Length - 1; i++)
            {
                if (!string.Equals(segments[i], CASINO_PATH, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        private static Uri CreateCollapsedCasinoUri(Uri originalUri, string[] segments)
        {
            var lastSegment = segments[^1];
            var shortPath = string.Format(COLLAPSED_CASINO_PATH_FORMAT, lastSegment);

            return new Uri($"{originalUri.Scheme}://{originalUri.Host}:{originalUri.Port}{shortPath}");
        }
    }
}

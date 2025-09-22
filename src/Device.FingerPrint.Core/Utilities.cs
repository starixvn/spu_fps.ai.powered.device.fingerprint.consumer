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
        private const int _TotalRedisDB = 16;

        public static bool IsGuidDeviceId(string deviceCode)
        {
            Guid newGuid;

            return Guid.TryParse(deviceCode, out newGuid);
        }

        public static IpModel Ip2Long(this string ipAddress)
        {
            var isValid = IPAddress.TryParse(ipAddress, out IPAddress ipAd);

            if (!isValid)
            {
                return new IpModel
                {
                    IpNumber = -1,
                    IsIpV6 = false
                };
            }

            BigInteger ipnum = 0;

            byte[] addrBytes = ipAd.GetAddressBytes();

            if (BitConverter.IsLittleEndian)
            {
                var byteList = new List<byte>(addrBytes);
                byteList.Reverse();
                addrBytes = byteList.ToArray();
            }

            if (addrBytes.Length > 8)
            {
                //IPv6
                ipnum = BitConverter.ToUInt64(addrBytes, 8);
                ipnum <<= 64;
                ipnum += BitConverter.ToUInt64(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = ipnum,
                    IsIpV6 = true
                };
            }
            else
            {
                //IPv4
                ipnum = BitConverter.ToUInt32(addrBytes, 0);

                return new IpModel
                {
                    IpNumber = ipnum,
                    IsIpV6 = false
                };
            }
        }

        public static int ConvertBitArray2Int(BitArray bitArray)
        {
            var binString = ToBitString(bitArray);

            return Convert.ToInt32(binString, 2);
        }

        public static int[] ConvertInt2BitArray(int input)
        {
            string s = Convert.ToString(input, 2);

            return s.PadLeft(8, '0')
                         .Select(c => int.Parse(c.ToString()))
                         .ToArray();
        }

        public static string ToBitString(BitArray bits)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < bits.Count; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }

        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        public static string ConvertNA2Empty(string data)
        {
            return data != "N/A" ? data : null;
        }

        public static string SubStringUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            int cutIndex = url.IndexOfAny(new[] { '?', '#' });
            var cleanUrl = cutIndex != -1 ? url.Substring(0, cutIndex) : url;

            return CollapseCasinoPaths(cleanUrl)?.ToString();
        }

        private static Uri CollapseCasinoPaths(string rawUrl)
        {
            var uri = new Uri(rawUrl);
            var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 4)
            {
                return uri;
            }

            var first = segments[0];
            var last = segments[segments.Length - 1];

            for (var i = 1; i < segments.Length - 1; i++)
            {
                if (!segments[i].Equals("casino", StringComparison.OrdinalIgnoreCase))
                {
                    return uri;
                }
            }

            if (first == "casino" && last != "casino")
            {
                string shortPath = $"/casino/.../casino/{last}";
                return new Uri($"{uri.Scheme}://{uri.Host}:{uri.Port}{shortPath}");
            }

            return uri;
        }

        public static string StripUnicodeCharactersFromString(string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
            {
                return null;
            }

            return Regex.Replace(inputValue,
                                @"[^\u0000-\u007F]",
                                string.Empty,
                                RegexOptions.None,
                                TimeSpan.FromSeconds(1) // 1 second timeout
                            );
        }

        public static string EncodeCapturedData(DeviceInfoModel deviceInfo)
        {
            return Cipher.Encrypt(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}#{1}#{2}#{3}#{4}",
                deviceInfo.Battery,
                deviceInfo.CapturedDate,
                deviceInfo.OSName,
                deviceInfo.TimeZone,
                deviceInfo.LocalIps));
        }
        public static bool IsBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return false;
            }

            if (base64String.Length % 4 != 0)
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

            #pragma warning disable S1449
            if (input.StartsWith(prefix))
            {
                return input.Replace(prefix, string.Empty);
            }
            #pragma warning restore S1449

            return input;
        }
    }
}

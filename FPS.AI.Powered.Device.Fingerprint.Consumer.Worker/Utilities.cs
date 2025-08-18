using Fanex.Logging;
using Fanex.Logging.Extensions;
using Fanex.Logging.Extensions.Database;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Encoders;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Collections;
using System.Net;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker
{
    public static class Utilities
    {
        private const int _TotalRedisDB = 16;
        public static string GetMD5HashFromObject<T>(ICrypto crypto, T value)
        {
            return crypto.GetMD5Hash(JsonConvert.SerializeObject(value));
        }

        public static bool IsValidJson(string data)
        {
            data = data.Trim();
            return (data.StartsWith("{") && data.EndsWith("}")) ||
                   (data.StartsWith("[") && data.EndsWith("]"));
        }

        public static void LogInfo(Exception ex)
        {
            LogService.Info(GetLogMessage(ex));
        }

        public static async Task LogInfoAsync(Exception ex)
        {
            await LogService.InfoAsync(GetLogMessage(ex));
        }

        public static void LogError(Exception ex)
        {
            LogService.Error(GetLogMessage(ex));
        }

        public static async Task LogErrorAsync(Exception ex)
        {
            await LogService.ErrorAsync(GetLogMessage(ex));
        }

        private static string GetLogMessage(Exception ex)
        {
            return new LogMessage(ex.Message)
                   .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                   .ToString();
        }

        public static bool ContainsUnicodeCharacter(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            return Regex.IsMatch(input, @"[^\u0000-\u007F]");
        }

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

        public static string ToBitString(BitArray bits)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < bits.Count; i++)
            {
                char c = bits[i] ? '1' : '0';
                sb.Append(c);
            }

            return sb.ToString();
        }

        public static string ConvertNA2Empty(string data)
        {
            return data != "N/A" ? data : null;
        }

        public static string SubStringUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            int cutIndex = url.IndexOfAny(new char[] { '?', '#' });
            var cleanUrl = cutIndex != -1 ? url.Substring(0, cutIndex) : url;

            return CollapseCasinoPaths(cleanUrl);
        }

        private static string CollapseCasinoPaths(string rawUrl)
        {
            var uri = new Uri(rawUrl);
            var segments = uri.AbsolutePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 4)
                return rawUrl;

            var first = segments[0];
            var last = segments[segments.Length - 1];

            for (var i = 1; i < segments.Length - 1; i++)
            {
                if (!segments[i].Equals("casino", StringComparison.OrdinalIgnoreCase))
                    return rawUrl;
            }

            if (first == "casino" && last != "casino")
            {
                string shortPath = $"/casino/.../casino/{last}";
                return $"{uri.Scheme}://{uri.Host}:{uri.Port}{shortPath}";
            }

            return rawUrl;
        }

        public static string StripUnicodeCharactersFromString(string inputValue)
        {
            if (string.IsNullOrEmpty(inputValue))
                return null;

            return Regex.Replace(inputValue, @"[^\u0000-\u007F]", string.Empty);
        }

        public static string EncodeCapturedData(DeviceInfoModel deviceInfo)
        {
            return Cipher.Encrypt(string.Format("{0}#{1}#{2}#{3}#{4}",
                deviceInfo.Battery,
                deviceInfo.CapturedDate,
                deviceInfo.OSName,
                deviceInfo.TimeZone,
                deviceInfo.LocalIps));
        }

        public static SessionKeyModel ParseSessionKey(string sessionKey, int defaultRedisDatabase)
        {
            if (sessionKey == null)
            {
                return null;
            }

            var parts = sessionKey.Split(':');
            var sessionKeyModel = new SessionKeyModel { RedisDbIndex = defaultRedisDatabase };

            if (parts.Length > 0)
            {
                sessionKeyModel.SessionKey = parts[0];
                if (parts.Length > 1 && int.TryParse(parts[1], out var parsedIndex))
                {
                    sessionKeyModel.RedisDbIndex = parsedIndex < _TotalRedisDB ? parsedIndex : defaultRedisDatabase;
                }
            }
            else
            {
                sessionKeyModel.SessionKey = sessionKey;
            }

            if (IsBase64(sessionKeyModel.SessionKey))
            {
                return sessionKeyModel;
            }

            return null;
        }
    }
}

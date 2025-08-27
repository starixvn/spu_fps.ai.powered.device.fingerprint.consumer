using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Model;
using System.Net;
using System.Numerics;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Services
{
    public static class Utilities
    {
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

        public static bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }
    }
}

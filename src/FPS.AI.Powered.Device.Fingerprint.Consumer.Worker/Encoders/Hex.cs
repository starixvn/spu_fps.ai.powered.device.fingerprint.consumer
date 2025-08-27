using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Encoders
{
    /// <summary>
    ///     Hex class for encoding and decoding.
    ///     <remarks>
    ///        <![CDATA[ http://code.openhub.net/file?fid=-UxxRWqDWT6bsedhL0XFisBQoJQ&cid=q9yJrXYbnFk&s=&fp=296521&mp&projSelected=true#L0 ]]>
    ///     </remarks>
    /// </summary>
    public static class Hex
    {
        private const string HexDigits = "0123456789abcdef";

        /// <summary>
        ///     Hex encoder.
        /// </summary>
        /// <param name="data">The data for encoding.</param>
        /// <returns>The encoded data.</returns>
        public static string Encode(string data)
        {
            var map = new string[256];
            for (var i = 0; i < 256; i++)
            {
                map[i] = string.Concat(HexDigits[i >> 4], HexDigits[i & 15]);
            }

            var length = data.Length;
            var result = new List<string>();
            for (var i = 0; i < length; i++)
            {
                result.Add(map[Convert.ToInt32(data[i])]);
            }

            return string.Join(null, result);
        }

        /// <summary>
        ///     Hex decoder.
        /// </summary>
        /// <param name="data">The data for decoding.</param>
        /// <returns>The decoded data.</returns>
        public static string Decode(string data)
        {
            var map = new char[256];
            for (var i = 0; i < 256; i++)
            {
                map[Convert.ToInt32(string.Concat(HexDigits[i >> 4], HexDigits[i & 15]), 16)] = Convert.ToChar(i);
            }

            var length = data.Length;
            if (length % 2 != 0)
            {
                data = '0' + data;
            }

            var result = new List<char>();

            for (var i = 0; i < length; i += 2)
            {
                result.Add(map[Convert.ToInt32(data.Substring(i, 2), 16)]);
            }

            return new string(result.ToArray());
        }
    }
}
namespace Device.FingerPrint.Core.Encoders
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class Base64
    {
        public static string Decode(string encodedData)
        {
            var validEncodedData = ValidEncodedData(encodedData);

            byte[] bytes = Convert.FromBase64String(validEncodedData.Replace(" ", "+"));

            return Encoding.UTF8.GetString(bytes);
        }

        public static string Decode(string encodedData, string key)
        {
            var output = new StringBuilder();
            int chr1;
            int chr2;
            int chr3;
            int enc1;
            int enc2;
            int enc3;
            int enc4;
            int i = 0;

            encodedData = Regex.Replace(encodedData,
                                        "[^A-Za-z0-9\\+\\/\\=]",
                                        "",
                                        RegexOptions.None,
                                        TimeSpan.FromSeconds(1)
                                    );

            while (i < encodedData.Length)
            {
                enc1 = key.IndexOf(encodedData[i++]);
                enc2 = i < encodedData.Length ? key.IndexOf(encodedData[i++]) : 0;
                enc3 = i < encodedData.Length ? key.IndexOf(encodedData[i++]) : 0;
                enc4 = i < encodedData.Length ? key.IndexOf(encodedData[i++]) : 0;

                chr1 = (enc1 << 2) | (enc2 >> 4);
                chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                chr3 = ((enc3 & 3) << 6) | enc4;

                output.Append(Convert.ToChar(chr1));

                if (enc3 != 64)
                {
                    output.Append(Convert.ToChar(chr2));
                }
                if (enc4 != 64)
                {
                    output.Append(Convert.ToChar(chr3));
                }
            }

            string result = Utf8Decode(output.ToString());

            return result;
        }

        private static string Utf8Decode(string utfText)
        {
            var str = new StringBuilder();
            int i = 0;
            int c;
            int c2;
            int c3;

            while (i < utfText.Length)
            {
                c = utfText[i];

                if (c < 128)
                {
                    str.Append(Convert.ToChar(c));
                    i++;
                }
                else if ((c > 191) && (c < 224))
                {
                    c2 = utfText[i + 1];
                    str.Append(Convert.ToChar(((c & 31) << 6) | (c2 & 63)));
                    i += 2;
                }
                else
                {
                    c2 = utfText[i + 1];
                    c3 = utfText[i + 2];
                    str.Append(Convert.ToChar(((c & 15) << 12) | ((c2 & 63) << 6) | (c3 & 63)));
                    i += 3;
                }
            }

            return str.ToString();
        }

        private static string ValidEncodedData(string encodedData)
        {
            const string pattern = "^[A-Za-z0-9+/]*={0,2}$";

            // Add timeout to prevent ReDoS
            Match match = Regex.Match(
                encodedData,
                pattern,
                RegexOptions.None,
                TimeSpan.FromSeconds(1) // Adjust timeout as appropriate
            );

            if (!match.Success)
            {
                int indexOfFirstInvalidChar = encodedData.IndexOf(encodedData.First(c =>
                                                                    !Regex.IsMatch(
                                                                        c.ToString(),
                                                                        "[A-Za-z0-9+/=]",
                                                                        RegexOptions.None,
                                                                        TimeSpan.FromSeconds(1) // set timeout
                                                                    )
                                                                )
                                                            );
                encodedData = encodedData.Substring(0, indexOfFirstInvalidChar);
            }

            return encodedData;
        }
    }
}

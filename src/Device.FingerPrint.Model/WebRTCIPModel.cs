namespace Device.FingerPrint.Model
{
    using Newtonsoft.Json;
    using System;
    using System.Security.Cryptography;
    using System.Text;

    [JsonConverter(typeof(WebRTCIPModelJsonConverter))]
    public sealed class WebRTCIPModel
    {
        public string Details { get; }
        public string Code { get; }

        public WebRTCIPModel(string details)
        {
            Details = details ?? string.Empty;
            Code = GenerateHashCode(Details) ?? string.Empty;
        }

        private static string? GenerateHashCode(string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return null;
            }

            using var md5 = MD5.Create();
            byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public class WebRTCIPModelJsonConverter : JsonConverter<WebRTCIPModel>
    {
        public override void WriteJson(JsonWriter writer, WebRTCIPModel? value, JsonSerializer serializer)
        {
            // Serialize only Details as raw string
            writer.WriteValue(value?.Details ?? string.Empty);
        }

        public override WebRTCIPModel ReadJson(JsonReader reader, Type objectType, WebRTCIPModel? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // Expecting a raw string in JSON
            var details = reader.Value?.ToString() ?? string.Empty;
            return new WebRTCIPModel(details);
        }
    }

}

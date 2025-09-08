namespace Device.FingerPrint.Core.Decoders
{
    using Device.FingerPrint.Core.Encoders;
    using Newtonsoft.Json;
    using System;

    public static class Base64JsonDecoder
    {
        private const string Base64Key = "wFKfVDGZqRbyYCBi6IveMaQAh1ntWcP8ujT/U7r2xXs+J0L=4d3zEHSo5glN9kOpm";

        /// <summary>
        /// Decodes Base64 string and deserializes to specified type
        /// </summary>
        /// <typeparam name="T">Target type to deserialize to</typeparam>
        /// <param name="encodedData">Base64 encoded string</param>
        /// <param name="key">Optional key for decoding (if null, uses default Base64 decode)</param>
        /// <returns>Deserialized object or null if decoding/deserialization fails</returns>
        public static T? DecodeAndDeserialize<T>(string? encodedData) where T : class
        {
            if (string.IsNullOrEmpty(encodedData))
            {
                return null;
            }

            try
            {
                string decodedValue = Base64.Decode(encodedData, Base64Key);

                if (string.IsNullOrEmpty(decodedValue))
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<T>(decodedValue, JsonSerializerSettingsFactory.Instance);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Decodes Base64 string and deserializes to specified type with validation
        /// </summary>
        /// <typeparam name="T">Target type to deserialize to</typeparam>
        /// <param name="encodedData">Base64 encoded string</param>
        /// <param name="key">Optional key for decoding (if null, uses default Base64 decode)</param>
        /// <param name="validateJson">Function to validate the decoded JSON string</param>
        /// <returns>Deserialized object or null if decoding/deserialization fails</returns>
        public static T? DecodeAndDeserialize<T>(string? encodedData, Func<string, bool> validateJson) where T : class
        {
            if (string.IsNullOrEmpty(encodedData))
            {
                return null;
            }

            try
            {
                string decodedValue = Base64.Decode(encodedData, Base64Key);

                if (string.IsNullOrEmpty(decodedValue) || !validateJson(decodedValue))
                {
                    return null;
                }

                return JsonConvert.DeserializeObject<T>(decodedValue, JsonSerializerSettingsFactory.Instance);
            }
            catch
            {
                return null;
            }
        }
    }
}

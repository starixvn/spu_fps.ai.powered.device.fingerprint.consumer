namespace Device.FingerPrint.Core
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;

    public static class JsonSerializerSettingsFactory
    {
        private static readonly JsonSerializerSettings _instance = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static JsonSerializerSettings Instance => _instance;
    }
}

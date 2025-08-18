namespace Device.FingerPrint.Core
{
    using Newtonsoft.Json.Serialization;
    using Newtonsoft.Json;

    public static class JsonSerializerSettingsFactory
    {
        private static volatile JsonSerializerSettings _instance;

        public static JsonSerializerSettings Instance => _instance;

        static JsonSerializerSettingsFactory()
        {
            _instance = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }
    }
}

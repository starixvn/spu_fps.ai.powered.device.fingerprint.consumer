using System.Text.Json.Serialization;

namespace Device.FingerPrint.Model
{
    public class ClientTransactionExceptionModel
    {
        [JsonPropertyName("fn")]
        public string FunctionName { get; set; }

        [JsonPropertyName("ex")]
        public string Exception { get; set; }

        public string UserAgent { get; set; }

        [JsonPropertyName("sv")]
        public string ScriptVersion { get; set; }

        [JsonPropertyName("au")]
        public string ApiUrl { get; set; }

        [JsonPropertyName("hn")]
        public string HostName { get; set; }

        [JsonPropertyName("sk")]
        public string SessionKey { get; set; }

        [JsonPropertyName("dc")]
        public string DeviceCode { get; set; }

        [JsonPropertyName("nc")]
        public string NewCookie { get; set; }

        public string RealIP { get; set; }

        public string FakeIP { get; set; }

    }
}

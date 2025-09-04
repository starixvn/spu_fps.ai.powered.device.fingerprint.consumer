namespace Device.FingerPrint.QueueConsumer.Models.Fingerprint
{
    public class AudioGroupModel
    {
        public AudioModel? Audio { get; set; }
        public object? AudioBaseLatency { get; set; } // number | string
    }

    public class AudioModel
    {
        public int? Audio { get; set; }
        public string? NoiseType { get; set; } // "clean" | "block" | "spoof" | "randomize"
    }

    public class AudioBaseLatencyModel
    {
        public object? AudioBaseLatency { get; set; } // number | string
    }
}

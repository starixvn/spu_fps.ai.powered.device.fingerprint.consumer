using System.Collections.Generic;

namespace Device.FingerPrint.QueueConsumer.Models
{
    public class FingerprintLogstashModel
    {
        public List<string> FingerprintAttributes { get; set; }

        public List<string> UnhandledAttributes { get; set; }
    }
}

using Device.FingerPrint.Model;
using System.Collections.Generic;

namespace Device.FingerPrint.QueueConsumer.Models
{
    public class TransactionLogstashModel
    {
        public RawTransactionModel Message { get; set; }

        public string Tags { get; set; }

        public List<string> FingerprintAttributes { get; set; }

        public List<string> UnhandledAttributes { get; set; }
    }
}

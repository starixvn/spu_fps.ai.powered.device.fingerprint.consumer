using System;

namespace Device.FingerPrint.Features.Monitor.Model
{
    public class TransactionSumEntity
    {
        public string mdyDate { get; set; }

        public int SubscriberId { get; set; }

        public int CountTrans { get; set; }
    }
}

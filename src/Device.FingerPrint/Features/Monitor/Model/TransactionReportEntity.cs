using System;

namespace Device.FingerPrint.Features.Monitor.Model
{
    public class TransactionReportEntity
    {
        public DateTime TransDate { get; set; }

        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public int TotalTrans { get; set; }

        public int TotalValidTrans { get; set; }
    }
}

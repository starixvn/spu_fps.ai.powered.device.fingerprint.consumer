using System;

namespace Device.FingerPrint.Model
{
    public class TransactionReportModel
    {
        public DateTime TransDate { get; set; }

        public int SubscriberId { get; set; }

        public string SubscriberName { get; set; }

        public int TotalTrans { get; set; }

        public int TotalValidTrans { get; set; }
    }
}

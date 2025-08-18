using System;

namespace Device.FingerPrint.Model.TransactionMonitoring
{
    public class TransactionMonitoringModel
    {
        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public DateTime TransDate { get; set; }

        public int TransTotal { get; set; }
    }
}

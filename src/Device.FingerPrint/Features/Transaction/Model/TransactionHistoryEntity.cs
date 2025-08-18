using System;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionHistoryEntity
    {
        public long TransID { get; set; }

        public DateTime TransTime { get; set; }

        public int SubscriberID { get; set; }

        public string UserName { get; set; }

        public long AccountID { get; set; }

        public string FirstDeviceCode { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string UrlDetails { get; set; }

        public string IP { get; set; }                

        public string RobotTracking { get; set; }        

        public long RawFingerPrintID { get; set; }

        public long RawUserAgentID { get; set; }
    }
}

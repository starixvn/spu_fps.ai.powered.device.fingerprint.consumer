namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionHistoryByDeviceItemModel
    {
        public ulong TransID { get; set; }

        public DateTime TransTime { get; set; }

        public string RegisterName { get; set; }

        public string UserName { get; set; }

        public ulong AccountID { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string OS { get; set; }

        public string Browser { get; set; }

        public string URLDetails { get; set; }

        public string IP { get; set; }

        public string RobotTracking { get; set; }
    }
}

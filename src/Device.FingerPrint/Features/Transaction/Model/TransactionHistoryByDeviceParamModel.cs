namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionHistoryByDeviceParamModel
    {
        public int SubscriberID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public ulong DeviceID { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}

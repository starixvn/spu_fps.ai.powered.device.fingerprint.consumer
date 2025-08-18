namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionParamModel
    {
        public int SubscriberID { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public ulong? CTSCustID { get; set; }

        public string? DeviceCode { get; set; }

        public string? IP { get; set; }

        public string? Country { get; set; }

        public string? Region { get; set; }

        public string? City { get; set; }

        public string? UserName { get; set; }

        public bool IsPartial { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}

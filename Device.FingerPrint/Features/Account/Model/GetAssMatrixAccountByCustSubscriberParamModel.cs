namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssMatrixAccountByCustSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public ulong CTSCustID { get; set; }

        public int NoOfCustomer { get; set; }

        public int NoOfDevice { get; set; }
    }
}

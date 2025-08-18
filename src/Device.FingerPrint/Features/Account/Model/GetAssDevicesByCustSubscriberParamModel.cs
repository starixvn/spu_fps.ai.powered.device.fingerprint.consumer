namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssDevicesByCustSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public ulong CTSCustID { get; set; }

        public int NoOfRecord { get; set; }
    }
}

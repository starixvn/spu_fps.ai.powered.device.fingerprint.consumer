namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAccountPerDeviceParamModel
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int SubscriberId { get; set; }

        public int TimeRange { get; set; }

        public int NoOfAccount { get; set; }
    }
}

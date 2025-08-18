namespace Device.FingerPrint.Features.Account.Model
{
    public class SearchAccountItemModel
    {
        public ulong CTSCustID { get; set; }

        public string Username { get; set; }

        public string RegisterName { get; set; } = string.Empty;

        public int SubscriberID { get; set; }

    }
}

namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAccountsPerDeviceResultModel
    {
        public IEnumerable<GetAccountsPerDeviceItemModel> Items { get; set; }

        public int TotalRows { get; set; }
    }
}

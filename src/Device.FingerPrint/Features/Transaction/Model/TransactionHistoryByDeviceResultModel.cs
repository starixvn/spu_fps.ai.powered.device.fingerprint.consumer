namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionHistoryByDeviceResultModel
    {
        public TransactionHistoryByDeviceResultModel()
        {
            this.Items = Enumerable.Empty<TransactionHistoryByDeviceItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionHistoryByDeviceItemModel> Items { get; set; }

        public int TotalItem { get; set; }

        public bool IsTimeout { get; set; }
    }
}

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionResultModel
    {
        public TransactionResultModel()
        {
            this.Items = Enumerable.Empty<TransactionItemModel>();
            this.TotalItem = 0;
        }

        public IEnumerable<TransactionItemModel> Items { get; set; }

        public int TotalItem { get; set; }

        public bool IsTimeout { get; set; }
    }
}

namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssMatrixAccountByCustSubscriberResultModel
    {
        public GetAssMatrixAccountByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssMatrixAccountByCustSubscriberItemModel>();
        }

        public IEnumerable<GetAssMatrixAccountByCustSubscriberItemModel> Items { get; set; }
    }
}

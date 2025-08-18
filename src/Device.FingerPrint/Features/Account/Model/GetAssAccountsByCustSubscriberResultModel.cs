namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssAccountsByCustSubscriberResultModel
    {
        public GetAssAccountsByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssAccountsByCustSubscriberItemModel>();
        }

        public IEnumerable<GetAssAccountsByCustSubscriberItemModel> Items { get; set; }
    }
}

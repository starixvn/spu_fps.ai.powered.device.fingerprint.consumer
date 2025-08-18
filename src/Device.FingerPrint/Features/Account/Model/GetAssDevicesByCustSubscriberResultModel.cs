namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssDevicesByCustSubscriberResultModel
    {
        public GetAssDevicesByCustSubscriberResultModel()
        {
            this.Items = Enumerable.Empty<GetAssDevicesByCustSubscriberItemModel>();
        }

        public IEnumerable<GetAssDevicesByCustSubscriberItemModel> Items { get; set; }
    }
}

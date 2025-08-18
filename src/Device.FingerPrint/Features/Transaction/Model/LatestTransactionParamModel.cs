namespace Device.FingerPrint.Features.Transaction.Model
{
    public class LatestTransactionParamModel
    {
        public int SubscriberID { get; set; }

        public int Length { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }

    }
}

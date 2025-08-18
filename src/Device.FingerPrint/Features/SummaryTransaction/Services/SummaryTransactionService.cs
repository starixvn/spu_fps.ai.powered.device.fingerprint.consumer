namespace Device.FingerPrint.Features.SummaryTransaction.Services
{

    public interface ISummaryTransactionService
    {

        void SummaryTransactionDaily(int batchSize);

        void SummaryRecoverTypeDaily(int batchSize);
    }
    public class SummaryTransactionService : ISummaryTransactionService
    {
        private readonly ISummaryTransactionDataAccess summaryTransactionDataAccess;

        public SummaryTransactionService(ISummaryTransactionDataAccess summaryTransactionDataAccess)
        {
            this.summaryTransactionDataAccess = summaryTransactionDataAccess;
        }

        public void SummaryTransactionDaily(int batchSize)
        {
            summaryTransactionDataAccess.SummaryTransactionDaily(batchSize);
        }

        public void SummaryRecoverTypeDaily(int batchSize)
        {
            summaryTransactionDataAccess.SummaryRecoverTypeDaily(batchSize);
        }
    }
}

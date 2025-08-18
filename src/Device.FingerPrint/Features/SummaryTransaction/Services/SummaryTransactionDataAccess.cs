using Device.FingerPrint.Features.SummaryTransaction.Criterias;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.SummaryTransaction.Services
{
    public interface ISummaryTransactionDataAccess
    {
        void SummaryTransactionDaily(int batchSize);

        void SummaryRecoverTypeDaily(int batchSize);
    }
    public class SummaryTransactionDataAccess : ISummaryTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public SummaryTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void SummaryTransactionDaily(int batchSize)
        {
            var criteria = new SummaryTransactionDailyCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }

        public void SummaryRecoverTypeDaily(int batchSize)
        {
            var criteria = new SummaryRecoverTypeDailyCriteria
            {
                BatchSize = batchSize
            };

            repository.Execute(criteria);
        }
    }
}

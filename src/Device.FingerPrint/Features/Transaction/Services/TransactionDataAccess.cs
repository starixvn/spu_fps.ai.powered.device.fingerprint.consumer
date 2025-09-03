using Device.FingerPrint.Features.Transaction.Criterias;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Services
{
    public interface ITransactionDataAccess
    {
        void InsertRawTransaction(string transJson);
    }

    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public TransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void InsertRawTransaction(string transJson)
        {
            var criteria = new InsertRawTransactionCriteria
            {
                RawTransJson = transJson,
            };

            repository.Execute(criteria);
        }
    }
}

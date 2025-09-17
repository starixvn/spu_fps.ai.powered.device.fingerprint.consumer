using Device.FingerPrint.Features.Transaction.Criterias;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Services
{
    public interface ITransactionDataAccess
    {
        void InsertRawTransaction(string transJson);

        void InsertFPUserAgent(string userAgentJson);

        void InsertFPFingerPrint(string fingerPrintJson);
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

        public void InsertFPUserAgent(string userAgentJson)
        {
            var criteria = new InsertFPUserAgentCriteria
            {
                UserAgentJson = userAgentJson
            };

            repository.Execute(criteria);
        }

        public void InsertFPFingerPrint(string fingerPrintJson)
        {
            var criteria = new InsertFPFingerPrintCriteria
            {
                FingerPrintJson = fingerPrintJson
            };

            repository.Execute(criteria);
        }
    }
}

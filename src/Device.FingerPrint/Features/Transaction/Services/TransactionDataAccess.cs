using Device.FingerPrint.Features.Transaction.Criterias;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Services
{
    public interface ITransactionDataAccess
    {
        void InsertRawFingerprint(string rawFPDetailJson);

        void InsertRawTransaction(string transJson);

        void InsertWebRTCIP(int detailsType, string webRTCJson);
    }

    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public TransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void InsertRawFingerprint(string rawFPDetailJson)
        {
            var criteria = new InsertRawFingerprintCriteria
            {
                RawFPDetails = rawFPDetailJson,
            };

            repository.Execute(criteria);
        }

        public void InsertRawTransaction(string transJson)
        {
            var criteria = new InsertRawTransactionCriteria
            {
                RawTransJson = transJson,
            };

            repository.Execute(criteria);
        }

        public void InsertWebRTCIP(int detailsType, string webRTCJson)
        {
            var criteria = new InsertRawDetailsCriteria
            {
                DetailsType = detailsType,
                Details = webRTCJson
            };

            repository.Execute(criteria);
        }
    }
}

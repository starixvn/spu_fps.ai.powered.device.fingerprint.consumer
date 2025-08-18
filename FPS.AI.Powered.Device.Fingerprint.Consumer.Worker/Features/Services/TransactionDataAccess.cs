using Fanex.Data.Repository;
using FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Features.Criterias;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Features.Services
{
    public interface ITransactionDataAccess
    {
        void InsertFPUserAgent(string userAgentJson, bool isTransOfDBExtra);

        void InsertFPFingerPrint(string fingerPrintJson, bool isTransOfDBExtra);
    }

    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public TransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void InsertFPUserAgent(string userAgentJson, bool isTransOfDBExtra)
        {
            var criteria = new InsertFPUserAgentCriteria
            {
                UserAgentJson = userAgentJson,
                IsTransOfDBExtra = isTransOfDBExtra
            };

            repository.Execute(criteria);
        }

        public void InsertFPFingerPrint(string fingerPrintJson, bool isTransOfDBExtra)
        {
            var criteria = new InsertFPFingerPrintCriteria
            {
                FingerPrintJson = fingerPrintJson,
                IsTransOfDBExtra = isTransOfDBExtra
            };

            repository.Execute(criteria);
        }
    }
}

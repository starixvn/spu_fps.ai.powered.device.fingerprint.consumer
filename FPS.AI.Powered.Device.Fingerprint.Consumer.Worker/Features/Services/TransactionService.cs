namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Features.Services
{
    public interface ITransactionService
    {
        void InsertFPUserAgent(string userAgentJson, bool isTransOfDBExtra);

        void InsertFPFingerPrint(string fingerPrintJson, bool isTransOfDBExtra);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ITransactionDataAccess transactionDataAccess;

        public TransactionService(ITransactionDataAccess transactionDataAccess)
        {
            this.transactionDataAccess = transactionDataAccess;
        }

        public void InsertFPUserAgent(string userAgentJson, bool isTransOfDBExtra)
        {
            transactionDataAccess.InsertFPUserAgent(userAgentJson, isTransOfDBExtra);
        }

        public void InsertFPFingerPrint(string fingerPrintJson, bool isTransOfDBExtra)
        {
            transactionDataAccess.InsertFPFingerPrint(fingerPrintJson, isTransOfDBExtra);
        }
    }
}

using Device.FingerPrint.Model;
using Newtonsoft.Json;

namespace Device.FingerPrint.Features.ClientTransaction.Services
{
    public interface IClientTransactionService
    {
        void InsertClientTransactionException(ClientTransactionExceptionModel logsData);
    }

    public class ClientTransactionService : IClientTransactionService
    {
        private readonly IClientTransactionDataAccess clientTransactionDataAccess;

        public ClientTransactionService(IClientTransactionDataAccess clientTransactionDataAccess)
        {
            this.clientTransactionDataAccess = clientTransactionDataAccess;
        }

        public void InsertClientTransactionException(ClientTransactionExceptionModel logsData)
        {
            var exceptionJson = $"[{JsonConvert.SerializeObject(logsData)}]";
            clientTransactionDataAccess.InsertClientTransactionException(exceptionJson);
        }
    }
}

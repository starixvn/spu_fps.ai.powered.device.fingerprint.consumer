using Fanex.Data.Repository;
using Device.FingerPrint.Features.ClientTransaction.Criterias;

namespace Device.FingerPrint.Features.ClientTransaction.Services
{
    public interface IClientTransactionDataAccess
    {
        void InsertClientTransactionException(string exceptionJson);
    }

    public class ClientTransactionDataAccess : IClientTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public ClientTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void InsertClientTransactionException(string exceptionJson)
        {
            var criteria = new InsertClientTransactionExceptionCriteria
            {
                ExceptionJson = exceptionJson
            };

            repository.Execute(criteria);
        }
    }
}

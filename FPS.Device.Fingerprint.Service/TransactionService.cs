using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPS.Device.Fingerprint.Service
{
    public interface ITransactionService
    {
        void InsertRawTransaction(IEnumerable<RawTransactionModel> transactions);
    }
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionDataAccess transactionDataAccess;

        public TransactionService(ITransactionDataAccess transactionDataAccess)
        {
            this.transactionDataAccess = transactionDataAccess;
        }

        public void InsertRawTransaction(IEnumerable<RawTransactionModel> transaction)
        {
            var transJson = JsonConvert.SerializeObject(TransformInsertRawTransaction(transactions));

            transactionDataAccess.InsertRawTransaction(transJson);

        }
    }
}

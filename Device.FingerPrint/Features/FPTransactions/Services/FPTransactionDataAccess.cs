namespace Device.FingerPrint.Features.FPTransactions.Services
{
    using Device.FingerPrint.Features.FPTransactions.Criterias;
    using Device.FingerPrint.Features.FPTransactions.Models;
    using Fanex.Data;
    using Fanex.Data.Repository;    
    using System.Collections.Generic;    

    public interface IFPTransactionDataAccess
    {
        IEnumerable<FPRawTransactionEntity> GetFPRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch);

        void InsertUserAgent(long fromId, long toId);

        void InsertTransaction(long fromId, long toId);

        void UnhandleSubscriberActionProcess(long fromId, long toId);
    }

    public class FPTransactionDataAccess : IFPTransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public FPTransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<FPRawTransactionEntity> GetFPRawTransaction(int isProcessed, int numberOfTrans, int numberOfBatch)
        {
            var criteria = new GetFPRawTransactionCriteria
            {
                IsProcessed = isProcessed,
                NumberOfTickets = numberOfTrans,
                NumberOfBatch = numberOfBatch
            };

            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                return db.Query<FPRawTransactionEntity>(criteria);
            }
        }

        public void InsertTransaction(long fromId, long toId)
        {
            var criteria = new InsertFPTransactionCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            repository.Execute(criteria);
        }

        public void InsertUserAgent(long fromId, long toId)
        {
            var criteria = new InsertFPUserAgentCriteria
            {
                FromStagingTransId = fromId,
                ToStagingTransId = toId
            };

            repository.Execute(criteria);
        }

        public void UnhandleSubscriberActionProcess(long fromId, long toId)
        {
            var criteria = new UnhandleFPSubscriberActionCriteria
            {
                FromTransId = fromId,
                ToTransId = toId
            };

            repository.Execute(criteria);
        }
    }
}

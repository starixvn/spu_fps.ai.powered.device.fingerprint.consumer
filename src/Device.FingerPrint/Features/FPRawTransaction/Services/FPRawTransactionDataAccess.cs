using Device.FingerPrint.Features.FPRawTransaction.Criteria;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.FPRawTransaction.Services
{
    public interface IFPRawTransactionDataAccess
    {
        int InsertTransaction(string jsonTrans);
    }

    public class FPRawTransactionDataAccess : IFPRawTransactionDataAccess
    {
        public FPRawTransactionDataAccess()
        {
        }

        public int InsertTransaction(string jsonTrans)
        {
            var repository = new DynamicRepository();

            var criteria = new FPCapturingTransactionCriteria
            {
                RawTrans = jsonTrans
            };

            return repository.Execute(criteria);
        }
    }
}

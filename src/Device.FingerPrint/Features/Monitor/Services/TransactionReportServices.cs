using Device.FingerPrint.Features.Monitor.Model;

namespace Device.FingerPrint.Features.Monitor.Services
{

    public interface ITransactionReportServices
    {
        IEnumerable<TransactionReportEntity> GetTransactionSumList(DateTime fromDate, DateTime toDate);
    }

    public class TransactionReportServices(ITransactionReportDataAccess dataAccess) : ITransactionReportServices
    {

        private readonly ITransactionReportDataAccess dataAccess = dataAccess;

        public IEnumerable<TransactionReportEntity> GetTransactionSumList(DateTime fromDate, DateTime toDate)
        {
            return dataAccess.GetTransactionSumList(fromDate, toDate);
        }
    }
}

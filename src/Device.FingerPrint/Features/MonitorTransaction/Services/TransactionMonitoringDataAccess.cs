
namespace Device.FingerPrint.Features.MonitorTransaction.Services
{
    using System.Threading.Tasks;
    using Device.FingerPrint.Features.MonitorTransaction.Criteria;
    using Fanex.Data.Repository;

    public interface ITransactionMonitoringDataAccess
    {
        Task<int> SumRawTransactionAsync(string transJson);
    }

    public class TransactionMonitoringDataAccess : ITransactionMonitoringDataAccess
    {
        public async Task<int> SumRawTransactionAsync(string transJson)
        {
            var repository = new DynamicRepository();

            var criteria = new SumRawTransactionCriteria
            {
                TransJson = transJson
            };

            return await repository.ExecuteAsync(criteria);
        }
    }
}

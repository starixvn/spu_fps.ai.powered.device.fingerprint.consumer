using Device.FingerPrint.Features.LogInfo.Criterias;
using Fanex.Data.Repository;
using System.Threading.Tasks;

namespace Device.FingerPrint.Features.LogInfo.Services
{
    public interface ILogInfoDataAccess
    {
        Task InsertDCSConsumerLogAsync(string logInfoJson);
    }

    public class LogInfoDataAccess : ILogInfoDataAccess
    {
        private readonly IDynamicRepository repository;

        public LogInfoDataAccess()
        {
            repository = new DynamicRepository();
        }

        public async Task InsertDCSConsumerLogAsync(string logInfoJson)
        {
            var criteria = new InsertDCSConsumerLogCriteria
            {
                ConsumerLogJson = logInfoJson
            };

            await repository.ExecuteAsync(criteria);
        }
    }
}

using System.Threading.Tasks;

namespace Device.FingerPrint.Features.LogTrail.Services
{
    public interface ILogTrailServices
    {
        Task<int> InsertSummaryLogTrailAsync(string rawLogEvent);
    }
    public class LogTrailServices : ILogTrailServices
    {
        private readonly ILogTrailDataAccess dataAccess;
        public LogTrailServices(ILogTrailDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        public async Task<int> InsertSummaryLogTrailAsync(string rawLogEvent)
        {
           return await dataAccess.InsertSummaryLogTrailAsync(rawLogEvent);
        }
    }
}

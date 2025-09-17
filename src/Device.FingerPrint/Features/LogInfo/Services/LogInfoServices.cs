using Device.FingerPrint.Features.LogInfo.Model;
using Newtonsoft.Json;

namespace Device.FingerPrint.Features.LogInfo.Services
{
    public interface ILogInfoServices
    {
        Task InsertDCSConsumerLogAsync(LogInfoModel logInfo);
    }

    public class LogInfoServices(ILogInfoDataAccess logInfoDataAccess) : ILogInfoServices
    {
        private readonly ILogInfoDataAccess logInfoDataAccess = logInfoDataAccess;

        public async Task InsertDCSConsumerLogAsync(LogInfoModel logInfo)
        {
            var logInfoJson = JsonConvert.SerializeObject(logInfo);
            logInfoJson = $"[{logInfoJson}]";
            await logInfoDataAccess.InsertDCSConsumerLogAsync(logInfoJson);
        }
    }
}

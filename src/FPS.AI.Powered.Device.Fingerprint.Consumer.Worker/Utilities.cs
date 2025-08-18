using Device.FingerPrint.Core.Services;
using Fanex.Logging.Extensions.Database;
using Fanex.Logging.Extensions;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Fanex.Logging;

namespace Device.FingerPrint.QueueConsumer
{
    public class Utilities
    {
        public static string GetMD5HashFromObject<T>(ICrypto crypto, T value)
        {
            return crypto.GetMD5Hash(JsonConvert.SerializeObject(value));
        }

        public static bool IsValidJson(string data)
        {
            data = data.Trim();
            return (data.StartsWith("{") && data.EndsWith("}")) ||
                   (data.StartsWith("[") && data.EndsWith("]"));
        }

        public static void LogInfo(Exception ex)
        {
            LogService.Info(GetLogMessage(ex));
        }

        public static async Task LogInfoAsync(Exception ex)
        {
            await LogService.InfoAsync(GetLogMessage(ex));
        }

        public static void LogError(Exception ex)
        {
            LogService.Error(GetLogMessage(ex));
        }

        public static async Task LogErrorAsync(Exception ex)
        {
            await LogService.ErrorAsync(GetLogMessage(ex));
        }

        private static string GetLogMessage(Exception ex)
        {
            return new LogMessage(ex.Message)
                   .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(ex))
                   .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                   .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(ex))
                   .ToString();
        }
    }
}

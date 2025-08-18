using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.LogInfo.Criterias
{
    public class InsertDCSConsumerLogCriteria : NonQueryCommand
    {

        [SpParam(SqlParamName = "ip_ConsumerLogJson", AllowNull = true)]
        public string ConsumerLogJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Log_DCSConsumerLog_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.SummaryTransaction.Criterias
{
    public class SummaryRecoverTypeDailyCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_batchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Summary_RecoverType_Daily";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

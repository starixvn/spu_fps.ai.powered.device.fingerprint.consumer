using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class GetForceMergeTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_Size")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "ip_Retry")]
        public int Retry { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Transform_ForceMergeTransaction_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

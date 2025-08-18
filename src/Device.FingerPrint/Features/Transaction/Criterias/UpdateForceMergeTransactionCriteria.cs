using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class UpdateForceMergeTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]
        public string Transaction { get; set; }


        public override string GetSettingKey()
        {
            return "FPS_FP_Transform_ForceMergeTransaction_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

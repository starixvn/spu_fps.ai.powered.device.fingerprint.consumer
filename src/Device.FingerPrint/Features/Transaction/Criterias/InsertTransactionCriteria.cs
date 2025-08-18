using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Transaction")]
        public string Transaction { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Transform_Transaction_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
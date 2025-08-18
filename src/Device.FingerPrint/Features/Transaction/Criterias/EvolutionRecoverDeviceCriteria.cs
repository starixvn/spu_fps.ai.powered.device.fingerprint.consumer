using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class EvolutionRecoverDeviceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]
        public string Transaction { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Recover_TransactionEvolution_RecoverDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
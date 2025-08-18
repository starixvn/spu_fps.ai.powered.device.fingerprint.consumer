using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class EvolutionNewDeviceCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]
        public string Transaction { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Recover_TransactionEvolution_NewDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
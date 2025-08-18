using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    class GetTransactionEvolutionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int Size { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Recover_TransactionEvolution_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

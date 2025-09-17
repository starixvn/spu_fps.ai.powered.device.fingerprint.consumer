using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Device.FingerPrint.Features.Monitor.Criterias
{
    public class GetTransactionSumCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Monitor_TransactionSum_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

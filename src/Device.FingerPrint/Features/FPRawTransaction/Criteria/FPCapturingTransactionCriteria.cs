using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.FPRawTransaction.Criteria
{
    class FPCapturingTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RawTransJson")]
        public string RawTrans { get; set; }

        public override string GetSettingKey() => "FP_DCS_DC_Transform_RawTransOld_Insert";

        public override bool IsValid() => true;
    }
}

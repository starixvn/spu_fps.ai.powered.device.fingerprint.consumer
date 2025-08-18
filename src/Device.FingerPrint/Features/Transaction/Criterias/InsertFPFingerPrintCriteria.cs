using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertFPFingerPrintCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FingerPrintJson", AllowNull = true)]
        public string FingerPrintJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsTransOfDBExtra { get; set; }

        public override string GetSettingKey()
        {
            return this.IsTransOfDBExtra ? "" : "DCS_DC_Transform_FP_FingerPrintInsert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

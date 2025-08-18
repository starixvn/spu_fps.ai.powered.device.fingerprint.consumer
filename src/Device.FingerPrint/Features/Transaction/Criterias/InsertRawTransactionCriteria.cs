using Fanex.Data.Repository;
using Fanex.Data;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertRawTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RawTransJson", AllowNull = true)]
        public string RawTransJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsTransOfDBExtra { get; set; }

        public override string GetSettingKey()
        {
            return this.IsTransOfDBExtra ? "DCS_ET_Transform_RawTrans_Insert" : "DCS_DC_Transform_RawTrans_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
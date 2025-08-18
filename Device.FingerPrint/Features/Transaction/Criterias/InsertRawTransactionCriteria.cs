using Fanex.Data.Repository;
using Fanex.Data;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    internal class InsertRawTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RawTransJson", AllowNull = true)]
        public string RawTransJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsTransOfDBExtra { get; set; }

        public override string GetSettingKey()
        {
            if (this.IsTransOfDBExtra)
            {
                return "DCS_ET_Transform_RawTrans_Insert";
            }
            else
            {
                return "DCS_DC_Transform_RawTrans_Insert";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
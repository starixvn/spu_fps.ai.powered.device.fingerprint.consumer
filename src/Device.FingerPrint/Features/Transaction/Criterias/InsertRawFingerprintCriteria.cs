using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertRawFingerprintCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_RawFPDetails", AllowNull = true)]
        public required string RawFPDetails { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_RawFingerprint_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertFPUserAgentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserAgentJson", AllowNull = true)]
        public string? UserAgentJson { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_FP_UserAgentInsert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

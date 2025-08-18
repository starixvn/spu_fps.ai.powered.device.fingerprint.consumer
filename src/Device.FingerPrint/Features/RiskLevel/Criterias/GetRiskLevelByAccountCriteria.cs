using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.RiskLevel.Criterias
{
    public class GetRiskLevelByAccountCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_AccountIDList", AllowNull = true)]
        public string AccountIDs { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustClassification_GetRiskLevelByAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

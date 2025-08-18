using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertJSChallengeInfoCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_JSChallengeInfoJson", AllowNull = true)]
        public string JSChallengeInfoJson { get; set; }

        [SpParam(Ignored = true)]
        public bool IsTransOfDBExtra { get; set; }

        public override string GetSettingKey()
        {
            if (this.IsTransOfDBExtra)
            {
                return "";
            }
            else
            {
                return "DCS_DC_Transform_JSChallengeInfo_Insert";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

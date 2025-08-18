using Fanex.Data;
using Fanex.Data.Repository;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Features.Criterias
{
    public class InsertFPUserAgentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_UserAgentJson", AllowNull = true)]
        public string UserAgentJson { get; set; }

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
                return "DCS_DC_Transform_FP_UserAgentInsert";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

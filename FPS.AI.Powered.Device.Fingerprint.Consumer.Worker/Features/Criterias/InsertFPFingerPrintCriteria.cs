using Fanex.Data;
using Fanex.Data.Repository;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Worker.Features.Criterias
{
    public class InsertFPFingerPrintCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FingerPrintJson", AllowNull = true)]
        public string FingerPrintJson { get; set; }

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
                return "DCS_DC_Transform_FP_FingerPrintInsert";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

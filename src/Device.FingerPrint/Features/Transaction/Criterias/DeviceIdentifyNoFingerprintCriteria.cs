using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class DeviceIdentifyNoFingerprintCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransList")]
        public string TransList { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Transform_DeviceIdentify_NoFingerPrint";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

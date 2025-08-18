using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.FPDevices.Criteria
{
    public class UpdateFPDeviceIdCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_TransJson")]
        public string TransIdJson { get; set; }

        public override string GetSettingKey()
        {
            return "FP_DCS_DC_Transform_Device_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

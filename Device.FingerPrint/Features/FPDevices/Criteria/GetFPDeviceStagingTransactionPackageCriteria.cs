using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.FPDevices.Criterias
{
    public class GetFPDeviceStagingTransactionPackageCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int Size { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NoOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "FP_DCS_DC_Transform_Device_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

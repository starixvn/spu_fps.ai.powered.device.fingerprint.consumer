using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Offset.Criterias
{
    public class UpdateOffsetCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Id")]
        public long Id { get; set; }

        [SpParam(SqlParamName = "ip_OffsetValue")]
        public long OffsetValue { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Core_Offset_UpdateOffsetValue";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

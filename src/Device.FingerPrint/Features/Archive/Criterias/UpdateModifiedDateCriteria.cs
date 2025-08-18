using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Archive.Criterias
{
    public class UpdateModifiedDateCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_BatchSize")]
        public int BatchSize { get; set; }

        public override string GetSettingKey()
        {
            return "FPS_FP_Archive_ModifiedDate_Update";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

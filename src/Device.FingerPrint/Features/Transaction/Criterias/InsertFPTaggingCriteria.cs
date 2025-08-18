using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertFPTaggingCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_Taggings", AllowNull = true)]
        public string Taggings { get; set; }

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
                return "DCS_DC_Transform_FP_TaggingInsert";
            }
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

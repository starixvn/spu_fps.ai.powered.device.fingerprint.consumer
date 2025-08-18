namespace Device.FingerPrint.Features.FPTransactions.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class InsertFPUserAgentCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID")]
        public long FromStagingTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID")]
        public long ToStagingTransId { get; set; }

        public override string GetSettingKey()
        {
            return "FP_DCS_DC_Transform_UserAgent_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

namespace Device.FingerPrint.Features.FPTransactions.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UnhandleFPSubscriberActionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_FromTransID")]
        public long FromTransId { get; set; }

        [SpParam(SqlParamName = "ip_ToTransID")]
        public long ToTransId { get; set; }

        public override string GetSettingKey()
        {
            return "FP_DCS_DC_Transform_UnhandleSubscriberAction";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

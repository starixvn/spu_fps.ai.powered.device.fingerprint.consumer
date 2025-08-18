namespace Device.FingerPrint.Features.Transaction.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetLastestTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_Length", AllowNull = true)]
        public int Length { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItem", AllowNull = true)]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transaction_GetLatestBySubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetTransactionHistoryBySubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserName", AllowNull = true)]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_IsPartial", AllowNull = true)]
        public bool IsPartial { get; set; }

        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItem", AllowNull = true)]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transaction_GetHistoryBySubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

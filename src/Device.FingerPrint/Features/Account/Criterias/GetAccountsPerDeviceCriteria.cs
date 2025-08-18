namespace Device.FingerPrint.Features.Account.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAccountsPerDeviceCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_LastDay", AllowNull = true)]
        public long TimeRange { get; set; }

        [SpParam(SqlParamName = "ip_NoOfAccount", AllowNull = true)]
        public int NoOfAccount { get; set; }

        [SpParam(SqlParamName = "ip_PageSize", AllowNull = true)]
        public int PageSize { get; set; }

        [SpParam(SqlParamName = "ip_PageIndex", AllowNull = true)]
        public int PageIndex { get; set; }

        [SpParam(SqlParamName = "op_TotalPages", AllowNull = true)]
        public int TotalPages { get; set; }

        [SpParam(SqlParamName = "op_TotalRows", AllowNull = true)]
        public int TotalRows { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Association_GetAccountsPerDevice";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

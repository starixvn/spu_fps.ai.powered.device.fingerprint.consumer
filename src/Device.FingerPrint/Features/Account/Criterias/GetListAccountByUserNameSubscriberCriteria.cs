namespace Device.FingerPrint.Features.Account.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetListAccountByUserNameSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserName", AllowNull = true)]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_Skip", AllowNull = true)]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take", AllowNull = true)]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItem", AllowNull = true)]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetPartialByUserNameSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

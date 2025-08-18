namespace Device.FingerPrint.Features.Account.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAccountByNameSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_UserName", AllowNull = true)]
        public string UserName { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_CustInfo_GetByNameSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

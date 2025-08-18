namespace Device.FingerPrint.Features.Account.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssAccountInfoByCustSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public ulong CTSCustID { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_DC_Association_GetAccountInfoByCustSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

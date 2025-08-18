namespace Device.FingerPrint.Features.Account.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssMatrixAccountByCustSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_CTSCustID", AllowNull = true)]
        public ulong CTSCustID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfCustomer", AllowNull = true)]
        public int NoOfCustomer { get; set; }

        [SpParam(SqlParamName = "ip_NoOfDevice", AllowNull = true)]
        public int NoOfDevice { get; set; }

        public override string GetSettingKey() => "CTS_DC_Association_GetAssociationMatrixByCustSubscriber";

        public override bool IsValid() => true;
    }
}

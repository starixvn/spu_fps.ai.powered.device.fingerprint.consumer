namespace Device.FingerPrint.Features.Devices.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssMatrixByDeviceSubsriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceID", AllowNull = true)]
        public ulong DeviceID { get; set; }

        [SpParam(SqlParamName = "ip_NoOfCustomer", AllowNull = true)]
        public int NoOfCustomer { get; set; }

        [SpParam(SqlParamName = "ip_NoOfDevice", AllowNull = true)]
        public int NoOfDevice { get; set; }

        public override string GetSettingKey() => "DCS_DC_Association_GetAssociationMatrixByDeviceSubscriber";

        public override bool IsValid() => true;
    }
}

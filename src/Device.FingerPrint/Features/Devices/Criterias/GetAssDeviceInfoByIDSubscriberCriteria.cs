namespace Device.FingerPrint.Features.Devices.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetAssDeviceInfoByIDSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceID", AllowNull = true)]
        public ulong DeviceID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Association_GetDeviceInfoByDeviceSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

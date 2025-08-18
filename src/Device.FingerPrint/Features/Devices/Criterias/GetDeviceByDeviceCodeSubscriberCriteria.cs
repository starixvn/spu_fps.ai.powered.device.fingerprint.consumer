namespace Device.FingerPrint.Features.Devices.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDeviceByDeviceCodeSubscriberCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceCode", AllowNull = true)]
        public string DeviceCode { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Device_GetByDeviceCodeSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

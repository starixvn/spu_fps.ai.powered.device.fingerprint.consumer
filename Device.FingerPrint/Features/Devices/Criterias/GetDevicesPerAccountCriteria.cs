namespace Device.FingerPrint.Features.Devices.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDevicesPerAccountCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_NoOfDevice", AllowNull = true)]
        public int NoOfDevice { get; set; }

        [SpParam(SqlParamName = "ip_LastDay", AllowNull = true)]
        public long TimeRange { get; set; }

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
            return "DCS_DC_Association_GetDevicesPerAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

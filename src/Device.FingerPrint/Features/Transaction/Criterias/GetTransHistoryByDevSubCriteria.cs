namespace Device.FingerPrint.Features.Transaction.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetTransHistoryByDevSubCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberID { get; set; }

        [SpParam(SqlParamName = "ip_DeviceID", AllowNull = true)]
        public ulong DeviceID { get; set; }

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
            return "DCS_DC_Transaction_GetHistoryByDeviceSubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

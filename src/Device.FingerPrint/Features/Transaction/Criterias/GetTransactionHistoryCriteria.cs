using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    class GetTransactionHistoryCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SubscriberID")]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_FromDate")]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate")]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_UserName")]
        public string UserName { get; set; }

        [SpParam(SqlParamName = "ip_IsPartial")]
        public bool IsPartial { get; set; }

        [SpParam(SqlParamName = "ip_Skip")]
        public int Skip { get; set; }

        [SpParam(SqlParamName = "ip_Take")]
        public int Take { get; set; }

        [SpParam(SqlParamName = "op_TotalItem")]
        public int TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transaction_GetHistoryBySubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

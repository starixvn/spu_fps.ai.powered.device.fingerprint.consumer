using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    class GetTransactionBySubscriberCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_TransID", AllowNull = true)]
        public long TransId { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int Limit { get; set; }
       

        public override string GetSettingKey()
        {
            return "DCS_DC_Transaction_GetTransactionBySubscriber";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

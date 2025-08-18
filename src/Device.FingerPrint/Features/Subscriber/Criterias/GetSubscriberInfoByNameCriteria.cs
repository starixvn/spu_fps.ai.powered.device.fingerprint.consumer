using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Subscriber.Criterias
{
    public class GetSubscriberInfoByNameCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_SubscriberName", AllowNull = true)]
        public string SubscriberName { get; set; }

        public override string GetSettingKey()
        {
            return "CTS_AD_Subscriber_GetInfoBySubscriberName";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

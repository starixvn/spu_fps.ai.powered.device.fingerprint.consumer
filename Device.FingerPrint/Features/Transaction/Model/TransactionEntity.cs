using Device.FingerPrint.Model;
using System;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionEntity
    {
        public string LoginName { get; set; }

        public int SubscriberId { get; set; }

        public string SessionKey { get; set; }

        public string ClientIP { get; set; }

        public string FakeIP { get; set; }

        public string UserAgent { get; set; }

        public string UserAgentCode { get; set; }

        public string Url { get; set; }

        public string Action { get; set; }

        public string TransKey { get; set; }

        public long CustId { get; set; }

        public long SubAccId { get; set; }

        public DateTime TransTime { get; set; }

        public string CookieCode { get; set; }

        public FPFingerprintAttributeModel FingerprintAttribute { get; set; }

        public UserAgentInfoModel UserAgentInfo { get; set; }

        public string FlaggedCode { get; set; }

        public string BotDetection { get; set; }

        public string BotComponent { get; set; }
    }
}

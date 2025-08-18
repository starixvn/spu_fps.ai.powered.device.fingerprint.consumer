using System;
using System.Numerics;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class TransactionInfoEntity
    {
        public ulong TransID { get; set; }

        public uint CustID { get; set; }

        public DateTime TransTime { get; set; }

        public ulong AccountID { get; set; }

        public string loginName { get; set; }

        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Flagged { get; set; }

        public string RobotTracking { get; set; }

        public string UserAgentKey { get; set; }

        public string UserAgent { get; set; }

        public string OSName { get; set; }

        public string OSVersion { get; set; }

        public string BrowserName { get; set; }

        public string BrowserVersion { get; set; }

        public string DeviceType { get; set; }

        public string IP { get; set; }

        public string IPID { get; set; }

        public ulong URLID { get; set; }

        public string URLDetails { get; set; }

        public ulong ActionResultID { get; set; }

        public string Action { get; set; }

        public string ActionResult { get; set; }

        public string TransStatus { get; set; }

        public DateTime InsertTime { get; set; }

        public uint BotDetectionValue { get; set; }

        public ulong BotComponentID { get; set; }

        public string FakeIP { get; set; }

        public ulong DeviceID { get; set; }

        public string DeviceCode { get; set; }

        public string FirstDeviceCode { get; set; }

        public int DeviceStatus { get; set; }

        public ulong DeviceFingerprintID { get; set; }

        public string FingerprintCode { get; set; }
    }
}

using System;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class DeviceIdentifyEntity
    {
        public ulong Id { get; set; }

        public DateTime TransTime { get; set; }

        public ulong RawFingerPrintId { get; set; }

        public ulong CookieId { get; set; }

        public ulong SubscriberId { get; set; }

        public ulong AccountId { get; set; }

        public string LoginName { get; set; }

        public ulong UserAgentId { get; set; }

        public long GroupId { get; set; }

        public int BatchNumber { get; set; }
    }
}

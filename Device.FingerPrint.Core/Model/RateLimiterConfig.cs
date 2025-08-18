namespace Device.FingerPrint.Core.Model
{
    public class RateLimiterConfig
    {
        public int PermitLimit { get; set; }
        public int TimeWindow { get; set; }
        public int QueueLimit { get; set; }
    }
}

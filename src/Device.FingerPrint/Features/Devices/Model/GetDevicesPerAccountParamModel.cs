namespace Device.FingerPrint.Features.Devices.Model
{
    public class GetDevicesPerAccountParamModel
    {
        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int SubscriberId { get; set; }

        public int TimeRange { get; set; }

        public int NoOfDevice { get; set; }
    }
}

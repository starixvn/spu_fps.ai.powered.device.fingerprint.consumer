namespace Device.FingerPrint.Features.Devices.Model
{
    public class GetAssDeviceInfoByIDSubscriberModel
    {
        public int AssociatedDevices { get; set; }

        public int AssociatedAccounts { get; set; }

        public IEnumerable<GetAssDeviceFirstSeen> FirstSeenBySubscriber { get; set; }
    }

    public class GetAssDeviceFirstSeen
    {
        public int SubscriberID { get; set; }

        public string SubscriberName { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}

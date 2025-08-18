namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssDevicesByCustSubscriberItemModel
    {
        public ulong DeviceID { get; set; }

        public string DeviceCode { get; set; }

        public DateTime FirstSeenDate { get; set; }

        public int AssociatedAccounts { get; set; }

        public int AssociatedDevices { get; set; }
    }
}

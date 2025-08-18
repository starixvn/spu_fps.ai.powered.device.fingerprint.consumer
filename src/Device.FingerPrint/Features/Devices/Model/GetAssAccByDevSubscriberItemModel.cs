namespace Device.FingerPrint.Features.Devices.Model
{
    public class GetAssAccByDevSubscriberItemModel
    {
        public ulong AccountID { get; set; }

        public string RegisterName { get; set; }

        public string UserName { get; set; }

        public int DeviceInCommon { get; set; }

        public int OtherDeives { get; set; }

        public int AssociatedAccount { get; set; }
    }
}

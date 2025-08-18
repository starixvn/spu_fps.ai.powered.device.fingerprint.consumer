namespace Device.FingerPrint.Features.Devices.Model
{
    public class GetAssMatrixByDeviceSubscriberParamModel
    {
        public int SubscriberID { get; set; }

        public ulong DeviceID { get; set; }

        public int NoOfCustomer { get; set; }

        public int NoOfDevice { get; set; }
    }
}

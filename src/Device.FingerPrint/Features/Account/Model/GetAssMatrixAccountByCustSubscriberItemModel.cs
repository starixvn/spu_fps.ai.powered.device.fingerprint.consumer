namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssMatrixAccountByCustSubscriberItemModel
    {
        public ulong CTSCustID { get; set; }

        public string UserName { get; set; }

        public string RegisterName { get; set; } = string.Empty;

        public ulong DeviceID { get; set; }

        public string FirstDeviceCode { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}

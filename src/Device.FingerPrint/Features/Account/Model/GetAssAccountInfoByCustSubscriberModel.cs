namespace Device.FingerPrint.Features.Account.Model
{
    public class GetAssAccountInfoByCustSubscriberModel
    {
        public int AssociatedDevices { get; set; }

        public int AssociatedAccounts { get; set; }

        public DateTime? FirstLogin { get; set; }

        public string FirstDevice { get; set; }
    }
}

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class AssociationByAccountAndSubscriberEntity
    {
        public IEnumerable<AssociationDeviceEntity> AssociationDevice { get; set; }

        public IEnumerable<AssociationAccountEntity> AssociationAccount { get; set; }
    }
}

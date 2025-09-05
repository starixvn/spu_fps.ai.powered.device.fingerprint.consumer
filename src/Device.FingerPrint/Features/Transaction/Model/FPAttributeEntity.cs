namespace Device.FingerPrint.Features.Transaction.Model
{
    public class FPAttributeEntity
    {
        public int FPAttributeID { get; set; }

        public string? AttributeName { get; set; }

        public string? AttributeShortName { get; set; }

        public string? GroupName { get; set; }

        public bool IsActive { get; set; }
    }
}

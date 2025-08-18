using System;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class AssociationAccountEntity
    {
        public long AccountID { get; set; }    
        
        public string Account { get; set; }

        public DateTime FirstAssociationDate { get; set; }
    }
}

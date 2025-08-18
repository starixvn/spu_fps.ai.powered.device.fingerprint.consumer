using System;

namespace Device.FingerPrint.Features.Transaction.Model
{
    public class AssociationDeviceEntity
    {
        public long DeviceID { get; set; }    
        
        public string Device { get; set; }

        public DateTime FirstSeenDate { get; set; }
    }
}

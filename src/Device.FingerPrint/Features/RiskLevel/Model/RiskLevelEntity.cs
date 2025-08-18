using System;

namespace Device.FingerPrint.Features.RiskLevel.Model
{
    public class RiskLevelEntity
    {
        public long Id { get; set; }

        public string RegisterName { get; set; }

        public string RiskLevel { get; set; }

        public long CustID { get; set; }

        public string CategoryName { get; set; }

        public string CustomerClassName { get; set; }

        public string CustomerClass { get; set; }

        public DateTime LastModifiedDate { get; set; }    

    }
}

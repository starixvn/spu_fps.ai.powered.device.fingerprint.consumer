using System;
using System.Collections.Generic;
using System.Text;

namespace Device.FingerPrint.Model
{
    public class RobotDetectionDetail
    {
        public float Score { get; set; }

        public int TestsRun { get; set; }

        public string TestsDetail { get; set; }
    }
}

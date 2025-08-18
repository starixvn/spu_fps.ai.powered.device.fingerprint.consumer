namespace Device.FingerPrint.Model
{
    public class JSChallengeInfo
    {
        public string ChallengeCode { get; set; }

        public string ChallengeIds { get; set; }

        public string Parameters { get; set; }

        public string Expect { get; set; }

        public string Expected { get; set; }

        public string Actual { get; set; }

        public int? ExeServerTimeInSecond { get; set; }

        public int? ExeClientTimeInSecond { get; set; }

        public int Result { get; set; }

        public string ErrMsg { get; set; }

    }
}

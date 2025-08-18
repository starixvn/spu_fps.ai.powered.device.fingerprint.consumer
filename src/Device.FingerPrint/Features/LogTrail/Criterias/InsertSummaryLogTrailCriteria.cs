namespace Device.FingerPrint.Features.LogTrail.Criterias
{
    using Fanex.Data.Repository;
    using Fanex.Data;
    public class InsertSummaryLogTrailCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_LogTrailJson")]
        public string RawLogEvent { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DT_Summary_LogTrail_Insert";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(RawLogEvent);
    }

}

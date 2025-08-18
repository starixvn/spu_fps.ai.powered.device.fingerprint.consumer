namespace Device.FingerPrint.Features.MonitorTransaction.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SumRawTransactionCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SumJson")]
        public string TransJson { get; set; }

        public override string GetSettingKey() => "DCS_DC_SumRawTransaction_Update";

        public override bool IsValid() => true;
    }
}

namespace Device.FingerPrint.Features.FPTransactions.Criterias
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetFPRawTransactionCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_IsProcessed")]
        public int IsProcessed { get; set; }

        [SpParam(SqlParamName = "ip_NoOfTickets")]
        public int NumberOfTickets { get; set; }

        [SpParam(SqlParamName = "ip_NoOfBatch")]
        public int NumberOfBatch { get; set; }

        public override string GetSettingKey()
        {
            return "FP_DCS_DC_Transform_RawTrans_GetPackage";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

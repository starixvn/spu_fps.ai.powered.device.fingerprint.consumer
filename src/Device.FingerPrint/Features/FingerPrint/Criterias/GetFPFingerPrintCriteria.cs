using Fanex.Data;
using Fanex.Data.Repository;
using System;

namespace Device.FingerPrint.Features.FingerPrint.Criterias
{
    public class GetFPFingerPrintCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "ip_FromDate", AllowNull = true)]
        public DateTime FromDate { get; set; }

        [SpParam(SqlParamName = "ip_ToDate", AllowNull = true)]
        public DateTime ToDate { get; set; }

        [SpParam(SqlParamName = "ip_BatchSize", AllowNull = true)]
        public int BatchSize { get; set; }

        [SpParam(SqlParamName = "ip_LastID", AllowNull = true)]
        public ulong LastID { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_OpenSearch_FPFingerPrint_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
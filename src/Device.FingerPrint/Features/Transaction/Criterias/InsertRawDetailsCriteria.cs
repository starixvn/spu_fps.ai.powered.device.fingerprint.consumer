// Licensed to the .NET Foundation under one or more agreements.

using Fanex.Data;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class InsertRawDetailsCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_DetailsType", AllowNull = true)]
        public required int DetailsType { get; set; }

        [SpParam(SqlParamName = "ip_Details", AllowNull = true)]
        public required string Details { get; set; }

        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_RawDetails_Insert";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
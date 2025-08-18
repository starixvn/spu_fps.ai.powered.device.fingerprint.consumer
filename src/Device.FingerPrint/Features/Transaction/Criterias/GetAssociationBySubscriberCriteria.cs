using Fanex.Data;
using Fanex.Data.Repository;
using System;
using System.Security.Permissions;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    class GetAssociationBySubscriberCriteria : NonQueryCommand
    {
        [SpParam(SqlParamName = "ip_SubscriberID", AllowNull = true)]
        public int SubscriberId { get; set; }

        [SpParam(SqlParamName = "ip_AccountName", AllowNull = true)]
        public string Account { get; set; }       

        public override string GetSettingKey()
        {
            return "DCS_DC_Association_GetAssociationByAccount";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}

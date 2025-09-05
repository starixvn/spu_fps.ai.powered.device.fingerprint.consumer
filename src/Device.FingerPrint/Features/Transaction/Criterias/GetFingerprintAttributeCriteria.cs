using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Criterias
{
    public class GetFingerprintAttributeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "DCS_DC_Transform_FingerprintAttribute_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
using Device.FingerPrint.Features.RiskLevel.Model;
using Device.FingerPrint.Features.Transaction.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Device.FingerPrint.Features.RiskLevel.Services
{
    public interface IRiskLevelService
    {
        IEnumerable<RiskLevelEntity> GetCustomerClassRiskLevel(int subscriberId, long lastId, int batchSize);

        IEnumerable<RiskLevelByAccountEntity> GetCustomerClassRiskLevelByAccount(string accountIDs);
    }

    public class RiskLevelService : IRiskLevelService
    {
        private readonly IRiskLevelDataAccess riskLevelAccess;
                
        public RiskLevelService(IRiskLevelDataAccess riskLevelAccess)
        {
            this.riskLevelAccess = riskLevelAccess;
        }

        public IEnumerable<RiskLevelEntity> GetCustomerClassRiskLevel(int subscriberId, long lastId, int batchSize)
        {
            return riskLevelAccess.GetCustomerClassRiskLevel(subscriberId, lastId, batchSize);
        }

        public IEnumerable<RiskLevelByAccountEntity> GetCustomerClassRiskLevelByAccount(string accountIDs)
        {
            return riskLevelAccess.GetCustomerClassRiskLevelByAccount(accountIDs);
        }
    }
}

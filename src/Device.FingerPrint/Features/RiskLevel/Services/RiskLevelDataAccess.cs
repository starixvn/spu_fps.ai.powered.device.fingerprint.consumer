using Device.FingerPrint.Features.RiskLevel.Criterias;
using Device.FingerPrint.Features.RiskLevel.Model;
using Device.FingerPrint.Features.Transaction.Criterias;
using Device.FingerPrint.Features.Transaction.Model;
using Device.FingerPrint.Model;
using Fanex.Data.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace Device.FingerPrint.Features.RiskLevel.Services
{
    public interface IRiskLevelDataAccess
    {
        IEnumerable<RiskLevelEntity> GetCustomerClassRiskLevel(int subscriberId, long lastId, int batchSize);
        IEnumerable<RiskLevelByAccountEntity> GetCustomerClassRiskLevelByAccount(string accountIDs);
    }

    public class RiskLevelDataAccess : IRiskLevelDataAccess
    {
        private readonly IDynamicRepository repository;

        public RiskLevelDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<RiskLevelEntity> GetCustomerClassRiskLevel(int subscriberId, long lastId, int batchSize)
        {
            var criteria = new GetLatestRiskLevelCriteria
            {
                 SubscriberId = subscriberId,
                 LastId = lastId,
                 BatchSize = batchSize
            };

            return repository.Fetch<RiskLevelEntity>(criteria);
        }

        public IEnumerable<RiskLevelByAccountEntity> GetCustomerClassRiskLevelByAccount(string accountIDs)
        {
            var criteria = new GetRiskLevelByAccountCriteria
            {
                AccountIDs = accountIDs
            };

            return repository.Fetch<RiskLevelByAccountEntity>(criteria);
        }
    }
}

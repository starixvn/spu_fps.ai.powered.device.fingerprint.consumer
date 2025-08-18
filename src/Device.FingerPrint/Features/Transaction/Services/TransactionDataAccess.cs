using Device.FingerPrint.Features.Transaction.Criterias;
using Device.FingerPrint.Features.Transaction.Model;
using Fanex.Data.Repository;

namespace Device.FingerPrint.Features.Transaction.Services
{
    public interface ITransactionDataAccess
    {
        void InsertRawTransaction(string transJson, bool isTransOfDBExtra);

        void InsertFPTagging(string taggings, bool isTransOfDBExtra);

        void InsertFPUserAgent(string userAgentJson, bool isTransOfDBExtra);

        void InsertJSChallengeInfo(string jsChallengeInfoJson, bool isTransOfDBExtra);

        void InsertFPFingerPrint(string fingerPrintJson, bool isTransOfDBExtra);

        void InsertTransaction(string transJson);

        void DeviceIdentify(string transJson, int groupType);

        void DeviceIdentifyNoFingerprint(string rawTransIds);

        (IEnumerable<DeviceIdentifyEntity>, IEnumerable<DeviceIdentifyEntity>, IEnumerable<ulong>) GetDeviceIdentify(int batchSize, int numberOfBatch);

        void UpdateMergeTransaction(string transJson);

        IEnumerable<MergeTransEntity> GetMergeTransaction(int batchSize, int retry);

        IEnumerable<MergeTransEntity> GetForceMergeTransaction(int batchSize, int retry);

        void UpdateForceMergeTransaction(string transJson);

        IEnumerable<TransactionEvolutionEntity> GetTransactionEvolution(int batchSize);

        IEnumerable<TransactionEvolutionAttributesEntity> GetTransactionEvolutionAttributes(string fingerPrintIds, string BrowserFamilyCode);

        void EvolutionNewDevice(string transJson);

        void EvolutionRecoverDevice(string transJson);

        IEnumerable<TransactionHistoryEntity> GetTransactionHistoryByTransIdSubscriber(int subscriberId, long transId, int limit);

        (IEnumerable<AssociationDeviceEntity>, IEnumerable<AssociationAccountEntity>) GetAssociationByAccountSubscriber(int subscriberId, string account);

        IEnumerable<AssociationAccountUsageEntity> GetAssociationBySubscriberDevice(int subscriberId, string deviceCode, DateTime fromDate, DateTime toDate);

        IEnumerable<TransactionInfoEntity> GetTransactionInfo(ulong lastTransID, int batchSize);

        TransactionResultModel GetTransactionByCustSubscriber(TransactionParamModel param);

        TransactionResultModel GetTransactionBySubscriber(TransactionParamModel param);

        LatestTransactionResultModel GetLastestTransactionBySubscriber(LatestTransactionParamModel param);

        TransactionHistoryByDeviceResultModel GetTransactionHistoryByDevice(TransactionHistoryByDeviceParamModel param);
    }

    public class TransactionDataAccess : ITransactionDataAccess
    {
        private readonly IDynamicRepository repository;

        public TransactionDataAccess()
        {
            repository = new DynamicRepository();
        }

        public void InsertRawTransaction(string transJson, bool isTransOfDBExtra)
        {
            var criteria = new InsertRawTransactionCriteria
            {
                RawTransJson = transJson,
                IsTransOfDBExtra = isTransOfDBExtra
            };

            repository.Execute(criteria);
        }

        public void InsertFPTagging(string taggings, bool isTransOfDBExtra)
        {
            var criteria = new InsertFPTaggingCriteria
            {
                Taggings = taggings,
                IsTransOfDBExtra = isTransOfDBExtra
            };

            repository.Execute(criteria);
        }

        public void InsertFPUserAgent(string userAgentJson, bool isTransOfDBExtra)
        {
            var criteria = new InsertFPUserAgentCriteria
            {
                UserAgentJson = userAgentJson,
                IsTransOfDBExtra = isTransOfDBExtra
            };

            repository.Execute(criteria);
        }

        public void InsertJSChallengeInfo(string jsChallengeInfoJson, bool isTransOfDBExtra)
        {
            var criteria = new InsertJSChallengeInfoCriteria
            {
                JSChallengeInfoJson = jsChallengeInfoJson,
                IsTransOfDBExtra = isTransOfDBExtra
            };

            repository.Execute(criteria);
        }

        public void InsertFPFingerPrint(string fingerPrintJson, bool isTransOfDBExtra)
        {
            var criteria = new InsertFPFingerPrintCriteria
            {
                FingerPrintJson = fingerPrintJson,
                IsTransOfDBExtra = isTransOfDBExtra
            };

            repository.Execute(criteria);
        }

        public void InsertTransaction(string transJson)
        {
            var criteria = new InsertTransactionCriteria
            {
                Transaction = transJson
            };

            repository.Execute(criteria);
        }

        public void DeviceIdentify(string transJson, int groupType)
        {
            var criteria = new DeviceIdentifyCriteria
            {
                TransJson = transJson,
                GroupType = groupType
            };

            repository.Execute(criteria);
        }

        public void DeviceIdentifyNoFingerprint(string rawTransIds)
        {
            var criteria = new DeviceIdentifyNoFingerprintCriteria
            {
                TransList = rawTransIds
            };

            repository.Execute(criteria);
        }

        public (IEnumerable<DeviceIdentifyEntity>, IEnumerable<DeviceIdentifyEntity>, IEnumerable<ulong>) GetDeviceIdentify(int batchSize, int numberOfBatch)
        {
            var criteria = new DeviceIdentifyGetCriteria
            {
                Size = batchSize,
                NumberOfBatch = numberOfBatch
            };

            return repository.FetchMultiple<DeviceIdentifyEntity, DeviceIdentifyEntity, ulong>(criteria);
        }

        public IEnumerable<TransactionEvolutionAttributesEntity> GetTransactionEvolutionAttributes(string fingerPrintIds, string BrowserFamilyCode)
        {
            var criteria = new GetTransactionAttributesEvolutionCriteria
            {
                FingerPrintIds = fingerPrintIds,
                BrowserFamilyCode = BrowserFamilyCode
            };

            return repository.Fetch<TransactionEvolutionAttributesEntity>(criteria);
        }

        public IEnumerable<TransactionEvolutionEntity> GetTransactionEvolution(int batchSize)
        {
            var criteria = new GetTransactionEvolutionCriteria
            {
                Size = batchSize
            };

            return repository.Fetch<TransactionEvolutionEntity>(criteria);
        }

        public IEnumerable<MergeTransEntity> GetForceMergeTransaction(int batchSize, int retry)
        {
            var criteria = new GetForceMergeTransactionCriteria
            {
                Size = batchSize,
                Retry = retry
            };

            return repository.Fetch<MergeTransEntity>(criteria);
        }

        public IEnumerable<MergeTransEntity> GetMergeTransaction(int batchSize, int retry)
        {
            var criteria = new GetMergeTransactionCriteria
            {
                Size = batchSize,
                Retry = retry
            };

            return repository.Fetch<MergeTransEntity>(criteria);
        }

        public void UpdateForceMergeTransaction(string transJson)
        {
            var criteria = new UpdateForceMergeTransactionCriteria
            {
                Transaction = transJson
            };

            repository.Execute(criteria);
        }

        public void UpdateMergeTransaction(string transJson)
        {
            var criteria = new UpdateMergeTransactionCriteria
            {
                Transaction = transJson
            };

            repository.Execute(criteria);
        }

        public void EvolutionNewDevice(string transJson)
        {
            var criteria = new EvolutionNewDeviceCriteria
            {
                Transaction = transJson
            };

            repository.Execute(criteria);
        }

        public void EvolutionRecoverDevice(string transJson)
        {
            var criteria = new EvolutionRecoverDeviceCriteria
            {
                Transaction = transJson
            };

            repository.Execute(criteria);
        }


        public IEnumerable<TransactionHistoryEntity> GetTransactionHistoryByTransIdSubscriber(int subscriberId, long transId, int limit)
        {
            var criteria = new GetTransactionBySubscriberCriteria
            {
                TransId = transId,
                Limit = limit,
                SubscriberId = subscriberId,
            };

            return repository.Fetch<TransactionHistoryEntity>(criteria);
        }

        public (IEnumerable<AssociationDeviceEntity>, IEnumerable<AssociationAccountEntity>) GetAssociationByAccountSubscriber(int subscriberId, string account)
        {
            var criteria = new GetAssociationBySubscriberCriteria
            {
                SubscriberId = subscriberId,
                Account = account
            };

            return repository.FetchMultiple<AssociationDeviceEntity, AssociationAccountEntity>(criteria);
        }

        public IEnumerable<AssociationAccountUsageEntity> GetAssociationBySubscriberDevice(int subscriberId, string deviceCode, DateTime fromDate, DateTime toDate)
        {
            var criteria = new GetAssociationBySubscriberDeviceCriteria
            {
                DeviceCode = deviceCode,
                FromDate = fromDate,
                ToDate = toDate,
                SubscriberId = subscriberId,
            };

            return repository.Fetch<AssociationAccountUsageEntity>(criteria);
        }

        public IEnumerable<TransactionInfoEntity> GetTransactionInfo(ulong lastTransID, int batchSize)
        {
            var criteria = new GetTransactionInfoCriteria
            {
                LastTransID = lastTransID,
                BatchSize = batchSize
            };

            return repository.Fetch<TransactionInfoEntity>(criteria);
        }

        public TransactionResultModel GetTransactionByCustSubscriber(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryByCustSubscriberCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public TransactionResultModel GetTransactionBySubscriber(TransactionParamModel param)
        {
            var data = new TransactionResultModel();

            var criteria = new GetTransactionHistoryBySubscriberCriteria
            {
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                UserName = param.UserName,
                IsPartial = param.IsPartial,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<TransactionItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public LatestTransactionResultModel GetLastestTransactionBySubscriber(LatestTransactionParamModel param)
        {
            var data = new LatestTransactionResultModel();

            var criteria = new GetLastestTransactionCriteria
            {
                SubscriberID = param.SubscriberID,
                Length = param.Length,
                Skip = param.Skip,
                Take = param.Take
            };

            data.Items = repository.Fetch<LatestTransactionItemModel>(criteria).Select(x => new LatestTransactionItemModel
            {
                Action = x.Action,
                ActionResult = x.ActionResult,
                Browser = x.Browser,
                CTSCustID = x.CTSCustID,
                FirstDeviceCode = x.FirstDeviceCode,
                IP = x.IP,
                OS = x.OS,
                RobotTracking = x.RobotTracking,
                SubscriberID = x.SubscriberID,
                TransID = x.TransID,
                TransTime = x.TransTime,
                URLDetails = x.URLDetails,
                UserName = x.UserName,
                RegisterName = x.RegisterName,
            });
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public TransactionHistoryByDeviceResultModel GetTransactionHistoryByDevice(TransactionHistoryByDeviceParamModel param)
        {
            var criteria = new GetTransHistoryByDevSubCriteria
            {
                DeviceID = param.DeviceID,
                FromDate = param.FromDate,
                ToDate = param.ToDate,
                SubscriberID = param.SubscriberID,
                Skip = param.Skip,
                Take = param.Take,
            };

            return new TransactionHistoryByDeviceResultModel
            {
                Items = repository.Fetch<TransactionHistoryByDeviceItemModel>(criteria),
                TotalItem = criteria.TotalItem
            };
        }
    }
}

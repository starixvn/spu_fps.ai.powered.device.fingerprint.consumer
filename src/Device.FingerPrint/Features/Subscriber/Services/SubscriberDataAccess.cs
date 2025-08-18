using Device.FingerPrint.Features.Subscriber.Criterias;
using Device.FingerPrint.Features.Subscriber.Model;
using Fanex.Data;
using Fanex.Data.Repository;
using System.Collections.Generic;

namespace Device.FingerPrint.Features.Subscriber.Services
{
    public interface ISubscriberDataAccess
    {
        SubscriberEntity GetSubscriberByName(string subscriberName);

        IEnumerable<SubscriberEntity> GetSubscriberList();
    }
    public class SubscriberDataAccess : ISubscriberDataAccess
    {
        private readonly IDynamicRepository repository;

        public SubscriberDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<SubscriberEntity> GetSubscriberList()
        {
            var criteria = new GetSubscriberListCriteria();

            return repository.Fetch<SubscriberEntity>(criteria);
        }

        public SubscriberEntity GetSubscriberByName(string subscriberName)
        {
            var criteria = new GetSubscriberInfoByNameCriteria
            {
                SubscriberName = subscriberName
            };

            using var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey());
            return db.QueryEntity<SubscriberEntity>(criteria);
        }
    }
}

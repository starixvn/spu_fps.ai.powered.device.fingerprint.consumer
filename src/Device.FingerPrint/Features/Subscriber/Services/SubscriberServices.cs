using Device.FingerPrint.Model;
using System.Collections.Generic;
using System.Linq;

namespace Device.FingerPrint.Features.Subscriber.Services
{
    public interface ISubscriberServices
    {
        SubscriberModel GetSubscriberByName(string subscriberName);

        IEnumerable<SubscriberModel> GetSubscriberList();
    }

    public class SubscriberServices : ISubscriberServices
    {

        private readonly ISubscriberDataAccess subscriberDataAccess;

        public SubscriberServices(ISubscriberDataAccess subscriberDataAccess)
        {
            this.subscriberDataAccess = subscriberDataAccess;
        }

        public SubscriberModel GetSubscriberByName(string subscriberName)
        {
            var subcribers = subscriberDataAccess.GetSubscriberByName(subscriberName);

            if (subcribers != null)
            {
                return new SubscriberModel
                {
                    SubscriberId = subcribers.SubscriberId,
                    SubscriberName = subcribers.SubscriberName
                };
            }

            return null;
        }

        public IEnumerable<SubscriberModel> GetSubscriberList()
        {
            var subcribers = subscriberDataAccess.GetSubscriberList();

            if (subcribers != null && subcribers.Any())
            {
                return subcribers.Select(x => new SubscriberModel
                {
                    SubscriberId = x.SubscriberId,
                    SubscriberName = x.SubscriberName
                });
            }

            return new List<SubscriberModel>();
        }
    }
}

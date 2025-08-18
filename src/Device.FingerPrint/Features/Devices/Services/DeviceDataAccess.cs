namespace Device.FingerPrint.Features.Devices.Services
{
    using Device.FingerPrint.Features.Devices.Criterias;
    using Device.FingerPrint.Features.Devices.Model;
    using Fanex.Data.Repository;

    public interface IDeviceDataAccess
    {
        IEnumerable<SearchDeviceItemModel> SearchDevice(SearchDeviceParamModel param);

        GetAssDeviceInfoByIDSubscriberModel GetAssDeviceInfoByIDSubscriber(GetAssDeviceInfoByIDSubscriberParamModel param);

        IEnumerable<GetAssAccByDevSubscriberItemModel> GetAssAccByDevSubscriber(GetAssAccByDevSubscriberParamModel param);

        IEnumerable<GetAssDevByDevSubscriberItemModel> GetAssDevByDevSubscriber(GetAssDevByDevSubscriberParamModel param);

        IEnumerable<GetAssMatrixByDeviceSubscriberItemModel> GetAssMatrixByDeviceSubscriber(GetAssMatrixByDeviceSubscriberParamModel param);

        GetDevicesPerAccountResultModel GetDevicesPerAccount(GetDevicesPerAccountParamModel param);
    }

    public class DeviceDataAccess : IDeviceDataAccess
    {
        private readonly IDynamicRepository repository;

        public DeviceDataAccess()
        {
            repository = new DynamicRepository();
        }

        public IEnumerable<SearchDeviceItemModel> SearchDevice(SearchDeviceParamModel param)
        {
            var criteria = new GetDeviceByDeviceCodeSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                DeviceCode = param.DeviceCode
            };

            var items = repository.Fetch<SearchDeviceItemModel>(criteria);
            return items;
        }

        public GetAssDeviceInfoByIDSubscriberModel GetAssDeviceInfoByIDSubscriber(GetAssDeviceInfoByIDSubscriberParamModel param)
        {
            var criteria = new GetAssDeviceInfoByIDSubscriberCriteria
            {
                DeviceID = param.DeviceID,
                SubscriberID = param.SubscriberID,
            };

            var data = repository.FetchMultiple<GetAssDeviceInfoByIDSubscriberModel, GetAssDeviceFirstSeen>(criteria);
            var result = data.FirstResultSet.FirstOrDefault() ?? new GetAssDeviceInfoByIDSubscriberModel();
            result.FirstSeenBySubscriber = data.SecondResultSet;

            return result;
        }

        public IEnumerable<GetAssAccByDevSubscriberItemModel> GetAssAccByDevSubscriber(GetAssAccByDevSubscriberParamModel param)
        {
            var criteria = new GetAssAccByDevSubscriberCriteria
            {
                DeviceID = param.DeviceID,
                NoOfRecord = param.NoOfRecord,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssAccByDevSubscriberItemModel>(criteria);
        }

        public IEnumerable<GetAssDevByDevSubscriberItemModel> GetAssDevByDevSubscriber(GetAssDevByDevSubscriberParamModel param)
        {
            var criteria = new GetAssDevByDevSubscriberCriteria
            {
                DeviceID = param.DeviceID,
                NoOfRecord = param.NoOfRecord,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssDevByDevSubscriberItemModel>(criteria);
        }

        public IEnumerable<GetAssMatrixByDeviceSubscriberItemModel> GetAssMatrixByDeviceSubscriber(GetAssMatrixByDeviceSubscriberParamModel param)
        {
            var criteria = new GetAssMatrixByDeviceSubsriberCriteria
            {
                DeviceID = param.DeviceID,
                NoOfCustomer = param.NoOfCustomer,
                NoOfDevice = param.NoOfDevice,
                SubscriberID = param.SubscriberID,
            };

            return repository.Fetch<GetAssMatrixByDeviceSubscriberItemModel>(criteria);
        }

        public GetDevicesPerAccountResultModel GetDevicesPerAccount(GetDevicesPerAccountParamModel param)
        {
            var data = new GetDevicesPerAccountResultModel();

            var criteria = new GetDevicesPerAccountCriteria
            {
                SubscriberId = param.SubscriberId,
                TimeRange = param.TimeRange,
                NoOfDevice = param.NoOfDevice,
                PageSize = param.PageSize,
                PageIndex = param.PageIndex
            };

            data.Items = repository.Fetch<GetDevicesPerAccountItemModel>(criteria);
            data.TotalRows = criteria.TotalRows;
            return data;
        }
    }
}

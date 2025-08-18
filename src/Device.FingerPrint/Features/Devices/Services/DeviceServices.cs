namespace Device.FingerPrint.Features.Devices.Services
{
    using Device.FingerPrint.Features.Devices.Model;

    public interface IDeviceServices
    {
        IEnumerable<SearchDeviceItemModel> SearchDevice(SearchDeviceParamModel param);

        GetAssDeviceInfoByIDSubscriberModel GetAssDeviceInfoByIDSubscriber(GetAssDeviceInfoByIDSubscriberParamModel param);

        IEnumerable<GetAssAccByDevSubscriberItemModel> GetAssAccByDevSubscriber(GetAssAccByDevSubscriberParamModel param);

        IEnumerable<GetAssDevByDevSubscriberItemModel> GetAssDevByDevSubscriber(GetAssDevByDevSubscriberParamModel param);

        IEnumerable<GetAssMatrixByDeviceSubscriberItemModel> GetAssMatrixByDeviceSubscriber(GetAssMatrixByDeviceSubscriberParamModel param);

        GetDevicesPerAccountResultModel GetDevicesPerAccount(GetDevicesPerAccountParamModel param);
    }

    public class DeviceServices : IDeviceServices
    {
        private readonly IDeviceDataAccess deviceDataAccess;

        public DeviceServices(IDeviceDataAccess deviceDataAccess)
        {
            this.deviceDataAccess = deviceDataAccess;
        }

        public IEnumerable<SearchDeviceItemModel> SearchDevice(SearchDeviceParamModel param)
        {

            return deviceDataAccess.SearchDevice(param);
        }

        public GetAssDeviceInfoByIDSubscriberModel GetAssDeviceInfoByIDSubscriber(GetAssDeviceInfoByIDSubscriberParamModel param)
        {

            return deviceDataAccess.GetAssDeviceInfoByIDSubscriber(param);
        }

        public IEnumerable<GetAssAccByDevSubscriberItemModel> GetAssAccByDevSubscriber(GetAssAccByDevSubscriberParamModel param)
        {

            return deviceDataAccess.GetAssAccByDevSubscriber(param);
        }

        public IEnumerable<GetAssDevByDevSubscriberItemModel> GetAssDevByDevSubscriber(GetAssDevByDevSubscriberParamModel param)
        {

            return deviceDataAccess.GetAssDevByDevSubscriber(param);
        }

        public IEnumerable<GetAssMatrixByDeviceSubscriberItemModel> GetAssMatrixByDeviceSubscriber(GetAssMatrixByDeviceSubscriberParamModel param)
        {

            return deviceDataAccess.GetAssMatrixByDeviceSubscriber(param);
        }

        public GetDevicesPerAccountResultModel GetDevicesPerAccount(GetDevicesPerAccountParamModel param)
        {

            return deviceDataAccess.GetDevicesPerAccount(param);
        }
    }
}

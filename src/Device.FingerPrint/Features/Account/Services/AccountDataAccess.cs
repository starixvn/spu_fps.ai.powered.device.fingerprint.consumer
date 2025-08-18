namespace Device.FingerPrint.Features.Account.Services
{
    using Device.FingerPrint.Features.Account.Criterias;
    using Device.FingerPrint.Features.Account.Model;
    using Fanex.Data.Repository;

    public interface IAccountDataAccess
    {
        SearchAccountResultModel SearchPartialAccount(SearchAccountParamModel param);

        SearchAccountResultModel SearchExactAccount(SearchAccountParamModel param);

        GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber(GetAssAccountInfoByCustSubscriberParamModel param);

        GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber(GetAssAccountsByCustSubscriberParamModel param);

        GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber(GetAssDevicesByCustSubscriberParamModel param);

        GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber(GetAssMatrixAccountByCustSubscriberParamModel param);

        GetAccountsPerDeviceResultModel GetAccountsPerDevice(GetAccountPerDeviceParamModel param);
    }

    public class AccountDataAccess : IAccountDataAccess
    {
        private readonly IDynamicRepository repository;
        public AccountDataAccess()
        {
            repository = new DynamicRepository();
        }

        public SearchAccountResultModel SearchPartialAccount(SearchAccountParamModel param)
        {
            var data = new SearchAccountResultModel();

            var criteria = new GetListAccountByUserNameSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                UserName = param.UserName,
                Take = param.Take,
                Skip = param.Skip
            };

            data.Items = repository.Fetch<SearchAccountItemModel>(criteria);
            data.TotalItem = criteria.TotalItem;

            return data;
        }

        public SearchAccountResultModel SearchExactAccount(SearchAccountParamModel param)
        {
            var data = new SearchAccountResultModel();

            var criteria = new GetAccountByNameSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                UserName = param.UserName
            };

            data.Items = repository.Fetch<SearchAccountItemModel>(criteria);
            data.TotalItem = data.Items.Count();

            return data;
        }

        public GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            var criteria = new GetAssAccountInfoByCustSubscriberCriteria
            {
                CTSCustID = param.CTSCustID,
                SubscriberID = param.SubscriberID
            };

            return repository.Get<GetAssAccountInfoByCustSubscriberModel>(criteria);
        }

        public GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber(GetAssAccountsByCustSubscriberParamModel param)
        {
            var data = new GetAssAccountsByCustSubscriberResultModel();

            var criteria = new GetAssAccountsByCustSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssAccountsByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber(GetAssDevicesByCustSubscriberParamModel param)
        {
            var data = new GetAssDevicesByCustSubscriberResultModel();

            var criteria = new GetAssDevicesByCustSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfRecord = param.NoOfRecord
            };

            data.Items = repository.Fetch<GetAssDevicesByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            var data = new GetAssMatrixAccountByCustSubscriberResultModel();
            var criteria = new GetAssMatrixAccountByCustSubscriberCriteria
            {
                SubscriberID = param.SubscriberID,
                CTSCustID = param.CTSCustID,
                NoOfCustomer = param.NoOfCustomer,
                NoOfDevice = param.NoOfDevice
            };

            data.Items = repository.Fetch<GetAssMatrixAccountByCustSubscriberItemModel>(criteria);

            return data;
        }

        public GetAccountsPerDeviceResultModel GetAccountsPerDevice(GetAccountPerDeviceParamModel param)
        {
            var data = new GetAccountsPerDeviceResultModel();

            var criteria = new GetAccountsPerDeviceCriteria
            {
                SubscriberId = param.SubscriberId,
                TimeRange = param.TimeRange,
                NoOfAccount = param.NoOfAccount,
                PageSize = param.PageSize,
                PageIndex = param.PageIndex
            };

            data.Items = repository.Fetch<GetAccountsPerDeviceItemModel>(criteria);
            data.TotalRows = criteria.TotalRows;
            return data;
        }
    }
}

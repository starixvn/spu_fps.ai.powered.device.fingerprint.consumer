namespace Device.FingerPrint.Features.Account.Services
{
    using Device.FingerPrint.Features.Account.Model;

    public interface IAccountServices
    {
        SearchAccountResultModel SearchAccount(SearchAccountParamModel param);

        GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber(GetAssAccountInfoByCustSubscriberParamModel param);

        GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber(GetAssAccountsByCustSubscriberParamModel param);

        GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber(GetAssDevicesByCustSubscriberParamModel param);

        GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber(GetAssMatrixAccountByCustSubscriberParamModel param);

        GetAccountsPerDeviceResultModel GetAccountsPerDevice(GetAccountPerDeviceParamModel param);
    }
    public class AccountServices : IAccountServices
    {
        private readonly IAccountDataAccess accountDataAccess;

        public AccountServices(IAccountDataAccess accountDataAccess)
        {
            this.accountDataAccess = accountDataAccess;
        }

        public SearchAccountResultModel SearchAccount(SearchAccountParamModel param)
        {
            if (param.IsSearchPartial)
            {
                return accountDataAccess.SearchPartialAccount(param);
            }
            else
            {
                return accountDataAccess.SearchExactAccount(param);
            }
        }

        public GetAssAccountInfoByCustSubscriberModel GetAssAccountInfoByCustSubscriber(GetAssAccountInfoByCustSubscriberParamModel param)
        {
            return accountDataAccess.GetAssAccountInfoByCustSubscriber(param);
        }

        public GetAssAccountsByCustSubscriberResultModel GetAssAccountsByCustSubscriber(GetAssAccountsByCustSubscriberParamModel param)
        {
            return accountDataAccess.GetAssAccountsByCustSubscriber(param);
        }

        public GetAssDevicesByCustSubscriberResultModel GetAssDevicesByCustSubscriber(GetAssDevicesByCustSubscriberParamModel param)
        {
            return accountDataAccess.GetAssDevicesByCustSubscriber(param);
        }

        public GetAssMatrixAccountByCustSubscriberResultModel GetAssMatrixAccountByCustSubscriber(GetAssMatrixAccountByCustSubscriberParamModel param)
        {
            return accountDataAccess.GetAssMatrixAccountByCustSubscriber(param);
        }

        public GetAccountsPerDeviceResultModel GetAccountsPerDevice(GetAccountPerDeviceParamModel param)
        {
            return accountDataAccess.GetAccountsPerDevice(param);
        }
    }
}

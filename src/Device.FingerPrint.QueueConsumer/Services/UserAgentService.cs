using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using Device.FingerPrint.Model;
using DeviceDetectorNET;
using Fanex.Logging;
using System;

namespace Device.FingerPrint.QueueConsumer.Services
{
    public enum DeviceType
    {
        Other = 0,
        Desktop = 1, 
        Tablet = 2, 
        Mobile = 3
    }

    public interface IUserAgentService
    {
        public UserAgentInfoModel GetUserAgentInfo(string userAgent);
    }

    public class UserAgentService : IUserAgentService
    {
        private readonly ICrypto crypto;
        private readonly IAppSettings appSettings;

        public UserAgentService(
            ICrypto crypto,
            IAppSettings appSettings)
        {
            this.crypto = crypto;
            this.appSettings = appSettings;
        }

        public UserAgentInfoModel GetUserAgentInfo(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                return new UserAgentInfoModel();
            }

            try
            {
                var deviceDetector = new DeviceDetector(userAgent);

                deviceDetector.SkipBotDetection();
                deviceDetector.Parse();

                var userAgentInfo = new UserAgentInfoModel { UserAgent = userAgent };

                var clientInfo = deviceDetector.GetClient();

                if (clientInfo != null && clientInfo.Success)
                {
                    userAgentInfo.ClientType = clientInfo.Match.Type;
                    userAgentInfo.ClientName = clientInfo.Match.Name;
                    userAgentInfo.ClientVersion = clientInfo.Match.Version;
                }

                var osInfo = deviceDetector.GetOs();

                if (osInfo != null && osInfo.Success)
                {
                    userAgentInfo.OSName = osInfo.Match.Name;
                    userAgentInfo.OSShortName = osInfo.Match.ShortName;
                    userAgentInfo.OSVersion = osInfo.Match.Version;
                    userAgentInfo.OSPlatform = osInfo.Match.Platform;
                }

                userAgentInfo.Device = deviceDetector.GetDeviceName();
                userAgentInfo.Brand = deviceDetector.GetBrandName();
                userAgentInfo.Model = deviceDetector.GetModel();
                userAgentInfo.DeviceType = (int)GetDeviceType(deviceDetector);

                userAgentInfo.FPUserAgentCode = crypto.GetMD5Hash(userAgent);

                return userAgentInfo;
            }
            catch(Exception ex)
            {
                _ = LogService.ErrorAsync($"{appSettings.Environment} - GetUserAgentInfo has error:\n {ex.Message}");
                return new UserAgentInfoModel();
            }
            
        }

    private static DeviceType GetDeviceType(DeviceDetector deviceDetector)
        {
            if (deviceDetector.IsDesktop())
            {
                return DeviceType.Desktop;
            }
            if (deviceDetector.IsTablet())
            {
                return DeviceType.Tablet;
            }
            if (deviceDetector.IsMobile())
            {
                return DeviceType.Mobile;
            }
            return DeviceType.Other;
        }
    }
}

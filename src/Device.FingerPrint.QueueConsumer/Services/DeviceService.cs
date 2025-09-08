// Licensed to the .NET Foundation under one or more agreements.

namespace Device.FingerPrint.QueueConsumer.Services
{
    using System;
    using Device.FingerPrint.Core.Decoders;
    using Device.FingerPrint.Model;
    using Device.FingerPrint.TransactionAPI.Model;


    public interface IDeviceService
    {
        DeviceInfoModel GetDeviceInfo(string? di, MessageQueue message);
    }

    public class DeviceService : IDeviceService
    {
        public DeviceInfoModel GetDeviceInfo(string? di, MessageQueue message)
        {
            try
            {
                if (IsInvalidDeviceCode(di))
                {
                    return TransformEmptyDeviceInfo();
                }

                var deviceInfo = DecodeAndDeserializeDeviceInfo(di);
                return deviceInfo ?? TransformEmptyDeviceInfo();
            }
            catch
            {
                return TransformEmptyDeviceInfo();
            }
        }

        private static bool IsInvalidDeviceCode(string? di)
        {
            return string.IsNullOrWhiteSpace(di) ||
                   string.Equals(di, "null", StringComparison.OrdinalIgnoreCase);
        }

        private static DeviceInfoModel TransformEmptyDeviceInfo()
        {
            return new DeviceInfoModel
            {
                Tagging = string.Empty,
                F4 = string.Empty,
                F5 = string.Empty,
                Rtc = string.Empty,
                Version = string.Empty
            };
        }


        private static DeviceInfoModel? DecodeAndDeserializeDeviceInfo(string? di)
        {
            return Base64JsonDecoder.DecodeAndDeserialize<DeviceInfoModel>(di);
        }
    }
}

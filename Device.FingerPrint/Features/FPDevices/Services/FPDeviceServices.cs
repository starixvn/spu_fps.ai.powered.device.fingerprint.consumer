using Device.FingerPrint.Features.FPDevices.Entities;
using Device.FingerPrint.Model;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Device.FingerPrint.Features.FPDevices.Services
{
    public interface IFPDeviceServices
    {
        IEnumerable<FPStagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch);

        DataResult UpdateDevices(IEnumerable<FPStagingTransIdEntity> stagingTransId);
    }

    public class FPDeviceServices : IFPDeviceServices
    {
        private readonly IFPDeviceDataAccess _deviceDataAcess;

        public FPDeviceServices(IFPDeviceDataAccess deviceDataAcess)
        {
            _deviceDataAcess = deviceDataAcess;
        }

        public IEnumerable<FPStagingTransIdEntity> GetStagingTransIdForDevice(int size, int noOfBatch)
        {
            if (size != 0 && noOfBatch != 0)
            {
                return _deviceDataAcess.GetFPStagingTransIdForDevice(size, noOfBatch);
            }

            return new List<FPStagingTransIdEntity>();
        }

        public DataResult UpdateDevices(IEnumerable<FPStagingTransIdEntity> stagingTransId)
        {

            if (stagingTransId != null && stagingTransId.Any())
            {
                var transIdJson = JsonConvert.SerializeObject(stagingTransId);

                return _deviceDataAcess.UpdateDevice(transIdJson);
            }

            return new DataResult
            {
                ErrorCode = -1,
                ErrorMessage = "Parameter is empty."
            };
        }
    }
}

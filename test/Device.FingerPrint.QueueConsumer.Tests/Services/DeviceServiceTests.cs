using Device.FingerPrint.QueueConsumer.Services;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class DeviceServiceTests
    {
        private readonly DeviceService _service;

        public DeviceServiceTests()
        {
            _service = new DeviceService();
        }
    }
}

using Device.FingerPrint.Core;
using Device.FingerPrint.QueueConsumer.Services;
using Moq;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class RawTransactionServiceTests
    {
        private readonly Mock<IBotDetectionService> _mockBotDetectionService;
        private readonly Mock<IAppSettings> _mockAppSettings;
        private readonly RawTransactionService _service;

        public RawTransactionServiceTests()
        {
            _mockBotDetectionService = new Mock<IBotDetectionService>();
            _mockAppSettings = new Mock<IAppSettings>();

            // Setup default values
            _mockAppSettings.Setup(x => x.Environment).Returns("Test");

            _service = new RawTransactionService(
                _mockBotDetectionService.Object,
                _mockAppSettings.Object);
        }

        [Fact]
        public void Constructor_Should_Create_Instance()
        {
            Assert.NotNull(_service);
        }
    }
}
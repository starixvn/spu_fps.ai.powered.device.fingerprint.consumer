using Xunit;
using FluentAssertions;
using Moq;
using Device.FingerPrint.Features.Devices.Services;
using Device.FingerPrint.Features.Devices.Model;
using System.Collections.Generic;

namespace Device.FingerPrint.Test
{
    public class DeviceServicesTests
    {
        [Fact]
        public void SearchDevice_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IDeviceDataAccess>();
            var service = new DeviceServices(mockDataAccess.Object);
            var param = new SearchDeviceParamModel { SubscriberID = 1, DeviceCode = "dev" };
            var expected = new List<SearchDeviceItemModel> { new SearchDeviceItemModel() } as IEnumerable<SearchDeviceItemModel>;
            mockDataAccess.Setup(x => x.SearchDevice(It.IsAny<SearchDeviceParamModel>())).Returns(expected);

            // Act
            var result = service.SearchDevice(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.SearchDevice(It.Is<SearchDeviceParamModel>(p => p.DeviceCode == "dev")), Times.Once);
        }

        [Fact]
        public void GetAssDeviceInfoByIDSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IDeviceDataAccess>();
            var service = new DeviceServices(mockDataAccess.Object);
            var param = new GetAssDeviceInfoByIDSubscriberParamModel { SubscriberID = 1 };
            var expected = new GetAssDeviceInfoByIDSubscriberModel();
            mockDataAccess.Setup(x => x.GetAssDeviceInfoByIDSubscriber(It.IsAny<GetAssDeviceInfoByIDSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssDeviceInfoByIDSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssDeviceInfoByIDSubscriber(It.Is<GetAssDeviceInfoByIDSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetAssAccByDevSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IDeviceDataAccess>();
            var service = new DeviceServices(mockDataAccess.Object);
            var param = new GetAssAccByDevSubscriberParamModel { SubscriberID = 1 };
            var expected = new List<GetAssAccByDevSubscriberItemModel> { new GetAssAccByDevSubscriberItemModel() } as IEnumerable<GetAssAccByDevSubscriberItemModel>;
            mockDataAccess.Setup(x => x.GetAssAccByDevSubscriber(It.IsAny<GetAssAccByDevSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssAccByDevSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssAccByDevSubscriber(It.Is<GetAssAccByDevSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetAssDevByDevSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IDeviceDataAccess>();
            var service = new DeviceServices(mockDataAccess.Object);
            var param = new GetAssDevByDevSubscriberParamModel { SubscriberID = 1 };
            var expected = new List<GetAssDevByDevSubscriberItemModel> { new GetAssDevByDevSubscriberItemModel() } as IEnumerable<GetAssDevByDevSubscriberItemModel>;
            mockDataAccess.Setup(x => x.GetAssDevByDevSubscriber(It.IsAny<GetAssDevByDevSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssDevByDevSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssDevByDevSubscriber(It.Is<GetAssDevByDevSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetAssMatrixByDeviceSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IDeviceDataAccess>();
            var service = new DeviceServices(mockDataAccess.Object);
            var param = new GetAssMatrixByDeviceSubscriberParamModel { SubscriberID = 1 };
            var expected = new List<GetAssMatrixByDeviceSubscriberItemModel> { new GetAssMatrixByDeviceSubscriberItemModel() } as IEnumerable<GetAssMatrixByDeviceSubscriberItemModel>;
            mockDataAccess.Setup(x => x.GetAssMatrixByDeviceSubscriber(It.IsAny<GetAssMatrixByDeviceSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssMatrixByDeviceSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssMatrixByDeviceSubscriber(It.Is<GetAssMatrixByDeviceSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetDevicesPerAccount_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IDeviceDataAccess>();
            var service = new DeviceServices(mockDataAccess.Object);
            var param = new GetDevicesPerAccountParamModel { SubscriberId = 1 };
            var expected = new GetDevicesPerAccountResultModel();
            mockDataAccess.Setup(x => x.GetDevicesPerAccount(It.IsAny<GetDevicesPerAccountParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetDevicesPerAccount(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetDevicesPerAccount(It.Is<GetDevicesPerAccountParamModel>(p => p.SubscriberId == 1)), Times.Once);
        }
    }
}

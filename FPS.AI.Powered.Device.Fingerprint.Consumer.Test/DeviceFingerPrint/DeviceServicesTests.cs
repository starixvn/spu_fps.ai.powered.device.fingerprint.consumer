using Xunit;
using FluentAssertions;
using Moq;
using Device.FingerPrint.Features.Devices.Services;
using Device.FingerPrint.Features.Devices.Model;
using System.Collections.Generic;

namespace FPS.AI.Powered.Device.Fingerprint.Test.DeviceFingerPrint
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
	}
}

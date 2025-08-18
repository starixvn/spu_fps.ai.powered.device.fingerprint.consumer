using Xunit;
using FluentAssertions;
using Moq;
using Device.FingerPrint.Features.Account.Services;
using Device.FingerPrint.Features.Account.Model;

namespace Device.FingerPrint.Test
{
    public class AccountServicesTests
    {
        [Fact]
        public void SearchAccount_WhenIsSearchPartialTrue_ShouldCallSearchPartialAccount()
        {
            // Arrange
            var mockDataAccess = new Mock<IAccountDataAccess>();
            var service = new AccountServices(mockDataAccess.Object);
            var param = new SearchAccountParamModel
            {
                SubscriberID = 1,
                UserName = "user",
                IsSearchPartial = true,
                Take = 10,
                Skip = 0
            };
            var expected = new SearchAccountResultModel();
            mockDataAccess.Setup(x => x.SearchPartialAccount(It.IsAny<SearchAccountParamModel>()))
                .Returns(expected);

            // Act
            var result = service.SearchAccount(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.SearchPartialAccount(It.Is<SearchAccountParamModel>(p => p.IsSearchPartial == true)), Times.Once);
            mockDataAccess.Verify(x => x.SearchExactAccount(It.IsAny<SearchAccountParamModel>()), Times.Never);
        }

        [Fact]
        public void SearchAccount_WhenIsSearchPartialFalse_ShouldCallSearchExactAccount()
        {
            // Arrange
            var mockDataAccess = new Mock<IAccountDataAccess>();
            var service = new AccountServices(mockDataAccess.Object);
            var param = new SearchAccountParamModel
            {
                SubscriberID = 1,
                UserName = "user",
                IsSearchPartial = false,
                Take = 10,
                Skip = 0
            };
            var expected = new SearchAccountResultModel();
            mockDataAccess.Setup(x => x.SearchExactAccount(It.IsAny<SearchAccountParamModel>()))
                .Returns(expected);

            // Act
            var result = service.SearchAccount(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.SearchExactAccount(It.Is<SearchAccountParamModel>(p => p.IsSearchPartial == false)), Times.Once);
            mockDataAccess.Verify(x => x.SearchPartialAccount(It.IsAny<SearchAccountParamModel>()), Times.Never);
        }

        [Fact]
        public void GetAssAccountInfoByCustSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IAccountDataAccess>();
            var service = new AccountServices(mockDataAccess.Object);
            var param = new GetAssAccountInfoByCustSubscriberParamModel { SubscriberID = 1 };
            var expected = new GetAssAccountInfoByCustSubscriberModel();
            mockDataAccess.Setup(x => x.GetAssAccountInfoByCustSubscriber(It.IsAny<GetAssAccountInfoByCustSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssAccountInfoByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssAccountInfoByCustSubscriber(It.Is<GetAssAccountInfoByCustSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetAssAccountsByCustSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IAccountDataAccess>();
            var service = new AccountServices(mockDataAccess.Object);
            var param = new GetAssAccountsByCustSubscriberParamModel { SubscriberID = 1 };
            var expected = new GetAssAccountsByCustSubscriberResultModel();
            mockDataAccess.Setup(x => x.GetAssAccountsByCustSubscriber(It.IsAny<GetAssAccountsByCustSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssAccountsByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssAccountsByCustSubscriber(It.Is<GetAssAccountsByCustSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetAssDevicesByCustSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IAccountDataAccess>();
            var service = new AccountServices(mockDataAccess.Object);
            var param = new GetAssDevicesByCustSubscriberParamModel { SubscriberID = 1 };
            var expected = new GetAssDevicesByCustSubscriberResultModel();
            mockDataAccess.Setup(x => x.GetAssDevicesByCustSubscriber(It.IsAny<GetAssDevicesByCustSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssDevicesByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssDevicesByCustSubscriber(It.Is<GetAssDevicesByCustSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetAssMatrixAccountByCustSubscriber_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IAccountDataAccess>();
            var service = new AccountServices(mockDataAccess.Object);
            var param = new GetAssMatrixAccountByCustSubscriberParamModel { SubscriberID = 1 };
            var expected = new GetAssMatrixAccountByCustSubscriberResultModel();
            mockDataAccess.Setup(x => x.GetAssMatrixAccountByCustSubscriber(It.IsAny<GetAssMatrixAccountByCustSubscriberParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAssMatrixAccountByCustSubscriber(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAssMatrixAccountByCustSubscriber(It.Is<GetAssMatrixAccountByCustSubscriberParamModel>(p => p.SubscriberID == 1)), Times.Once);
        }

        [Fact]
        public void GetAccountsPerDevice_ShouldDelegateToDataAccess()
        {
            // Arrange
            var mockDataAccess = new Mock<IAccountDataAccess>();
            var service = new AccountServices(mockDataAccess.Object);
            var param = new GetAccountPerDeviceParamModel { SubscriberId = 1 };
            var expected = new GetAccountsPerDeviceResultModel();
            mockDataAccess.Setup(x => x.GetAccountsPerDevice(It.IsAny<GetAccountPerDeviceParamModel>()))
                .Returns(expected);

            // Act
            var result = service.GetAccountsPerDevice(param);

            // Assert
            result.Should().BeSameAs(expected);
            mockDataAccess.Verify(x => x.GetAccountsPerDevice(It.Is<GetAccountPerDeviceParamModel>(p => p.SubscriberId == 1)), Times.Once);
        }
    }
}

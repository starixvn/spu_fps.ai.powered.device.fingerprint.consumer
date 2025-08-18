using Xunit;
using FluentAssertions;
using Moq;
using Device.FingerPrint.Features.Account.Services;
using Device.FingerPrint.Features.Account.Model;

namespace FPS.AI.Powered.Device.Fingerprint.Test.DeviceFingerPrint
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
	}
}

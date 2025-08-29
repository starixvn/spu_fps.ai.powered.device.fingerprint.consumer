using Xunit;
using Device.FingerPrint.QueueConsumer.Models;
using Device.FingerPrint.Model;
using Device.FingerPrint.Core.Services;
using Moq;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    public class FPNewFingerprintModelTests
    {
        [Fact]
        public void Should_Create_FPNewFingerprintModel_Instance()
        {
            var attributeModel = new FPFingerprintAttributeModel();
            var cryptoMock = new Mock<ICrypto>();
            var model = new FPNewFingerprintModel(attributeModel, cryptoMock.Object);
            Assert.NotNull(model);
        }

        [Fact]
        public void Should_Return_AttributeModel()
        {
            var attributeModel = new FPFingerprintAttributeModel();
            var cryptoMock = new Mock<ICrypto>();
            var model = new FPNewFingerprintModel(attributeModel, cryptoMock.Object);
            Assert.Equal(attributeModel, model.AttributeModel);
        }

        [Fact]
        public void Should_Return_Code()
        {
            var attributeModel = new FPFingerprintAttributeModel();
            var cryptoMock = new Mock<ICrypto>();
            // Giả lập Utilities.GetMD5HashFromObject trả về giá trị cố định
            // Sử dụng Moq để mock static method nếu cần, hoặc kiểm tra giá trị trả về là string
            var model = new FPNewFingerprintModel(attributeModel, cryptoMock.Object);
            var code = model.Code;
            Assert.IsType<string>(code);
        }
    }
}

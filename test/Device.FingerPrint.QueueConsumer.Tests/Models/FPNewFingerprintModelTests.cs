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
            cryptoMock.Setup(c => c.GetMD5Hash(It.IsAny<string>())).Returns("mocked_hash_value");

            var model = new FPNewFingerprintModel(attributeModel, cryptoMock.Object);
            var code = model.Code;
            Assert.IsType<string>(code);
            Assert.Equal("mocked_hash_value", code);
        }
    }
}

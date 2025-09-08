using Device.FingerPrint.QueueConsumer.Models;
using FluentAssertions;
using Xunit;

namespace Device.FingerPrint.QueueConsumer.Tests.Models
{
    public class MobileModelTests
    {
        [Fact]
        public void MobileModel_Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var mobileModel = new MobileModel();

            // Act
            mobileModel.MachineCode = "machine123";
            mobileModel.CustomCode = "custom456";
            mobileModel.MediaDRMCode = "drm789";
            mobileModel.IsMobileAppTrans = true;
            mobileModel.IsEmulator = false;
            mobileModel.ModelName = "iPhone 12";
            mobileModel.Manufacturer = "Apple";
            mobileModel.Brand = "Apple";
            mobileModel.SdkVersion = "15.0";
            mobileModel.OS = "iOS 15.0";
            mobileModel.Timezone = "UTC+7";
            mobileModel.SystemLanguage = "en";
            mobileModel.Country = "VN";
            mobileModel.DeviceDetails = "iPhone 12, iOS 15.0";

            // Assert
            mobileModel.MachineCode.Should().Be("machine123");
            mobileModel.CustomCode.Should().Be("custom456");
            mobileModel.MediaDRMCode.Should().Be("drm789");
            mobileModel.IsMobileAppTrans.Should().BeTrue();
            mobileModel.IsEmulator.Should().BeFalse();
            mobileModel.ModelName.Should().Be("iPhone 12");
            mobileModel.Manufacturer.Should().Be("Apple");
            mobileModel.Brand.Should().Be("Apple");
            mobileModel.SdkVersion.Should().Be("15.0");
            mobileModel.OS.Should().Be("iOS 15.0");
            mobileModel.Timezone.Should().Be("UTC+7");
            mobileModel.SystemLanguage.Should().Be("en");
            mobileModel.Country.Should().Be("VN");
            mobileModel.DeviceDetails.Should().Be("iPhone 12, iOS 15.0");
        }

        [Fact]
        public void MobileModel_BooleanProperties_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var mobileModel = new MobileModel();

            // Assert
            mobileModel.IsMobileAppTrans.Should().BeFalse(); // Default bool value
            mobileModel.IsEmulator.Should().BeFalse(); // Default bool value
        }

        [Fact]
        public void MobileModel_StringProperties_ShouldAcceptNullValues()
        {
            // Arrange & Act
            var mobileModel = new MobileModel
            {
                MachineCode = null,
                CustomCode = null,
                MediaDRMCode = null,
                ModelName = null,
                Manufacturer = null,
                Brand = null,
                SdkVersion = null,
                OS = null,
                Timezone = null,
                SystemLanguage = null,
                Country = null,
                DeviceDetails = null
            };

            // Assert
            mobileModel.MachineCode.Should().BeNull();
            mobileModel.CustomCode.Should().BeNull();
            mobileModel.MediaDRMCode.Should().BeNull();
            mobileModel.ModelName.Should().BeNull();
            mobileModel.Manufacturer.Should().BeNull();
            mobileModel.Brand.Should().BeNull();
            mobileModel.SdkVersion.Should().BeNull();
            mobileModel.OS.Should().BeNull();
            mobileModel.Timezone.Should().BeNull();
            mobileModel.SystemLanguage.Should().BeNull();
            mobileModel.Country.Should().BeNull();
            mobileModel.DeviceDetails.Should().BeNull();
        }

        [Fact]
        public void MobileModel_EmptyConstructor_ShouldCreateInstance()
        {
            // Arrange & Act
            var mobileModel = new MobileModel();

            // Assert
            mobileModel.Should().NotBeNull();
            mobileModel.Should().BeOfType<MobileModel>();
        }
    }
}

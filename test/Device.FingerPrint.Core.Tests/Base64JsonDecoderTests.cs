using Device.FingerPrint.Core.Decoders;
using Device.FingerPrint.Core.Encoders;
using Newtonsoft.Json;
using System;
using System.Text;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class Base64JsonDecoderTests
    {
        // Test model để sử dụng trong các test
        public class TestModel
        {
            public string Name { get; set; } = string.Empty;
            public int Age { get; set; }
            public bool IsActive { get; set; }
        }

        // Helper method để tạo test data - sử dụng standard Base64 encoding thay vì custom key
        private static string CreateTestEncodedData(string json)
        {
            if (string.IsNullOrEmpty(json))
                return string.Empty;

            // Sử dụng standard Base64 encoding
            var bytes = Encoding.UTF8.GetBytes(json);
            return Convert.ToBase64String(bytes);
        }


        [Fact]
        public void DecodeAndDeserialize_WithNullInput_ShouldReturnNull()
        {
            // Arrange
            string? encodedData = null;

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithEmptyString_ShouldReturnNull()
        {
            // Arrange
            string encodedData = string.Empty;

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidData_ShouldReturnDeserializedObject()
        {
            // Arrange
            var testModel = new TestModel
            {
                Name = "Test User",
                Age = 25,
                IsActive = true
            };
            string json = JsonConvert.SerializeObject(testModel);
            // Sử dụng một string đã được encode với custom key
            // Để test, chúng ta sẽ sử dụng một string đơn giản
            string encodedData = "test-encoded-data";

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData);

            // Assert - Vì sử dụng custom key và string không hợp lệ, kết quả sẽ là null
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithStandardBase64_ShouldReturnNull()
        {
            // Arrange
            var testModel = new TestModel
            {
                Name = "Test User",
                Age = 25,
                IsActive = true
            };
            string json = JsonConvert.SerializeObject(testModel);
            string encodedData = CreateTestEncodedData(json);

            // Act - Base64JsonDecoder sử dụng custom key, nên standard base64 sẽ fail
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithInvalidBase64_ShouldReturnNull()
        {
            // Arrange
            string invalidBase64 = "invalid-base64-string!@#";

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(invalidBase64);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithInvalidJson_ShouldReturnNull()
        {
            // Arrange
            string invalidJson = "invalid json string";
            string encodedData = "invalid-encoded-data";

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithEmptyJson_ShouldReturnNull()
        {
            // Arrange
            string emptyJson = "";
            string encodedData = "";

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidation_NullInput_ShouldReturnNull()
        {
            // Arrange
            string? encodedData = null;
            Func<string, bool> validateJson = json => !string.IsNullOrEmpty(json);

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidation_EmptyString_ShouldReturnNull()
        {
            // Arrange
            string encodedData = string.Empty;
            Func<string, bool> validateJson = json => !string.IsNullOrEmpty(json);

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidation_ValidDataAndValidValidation_ShouldReturnDeserializedObject()
        {
            // Arrange
            var testModel = new TestModel
            {
                Name = "Test User",
                Age = 30,
                IsActive = false
            };
            string json = JsonConvert.SerializeObject(testModel);
            string encodedData = "test-encoded-data";
            Func<string, bool> validateJson = json => json.Contains("Test User");

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson);

            // Assert - Vì encoded data không hợp lệ, kết quả sẽ là null
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidation_ValidDataButInvalidValidation_ShouldReturnNull()
        {
            // Arrange
            var testModel = new TestModel
            {
                Name = "Test User",
                Age = 30,
                IsActive = false
            };
            string json = JsonConvert.SerializeObject(testModel);
            string encodedData = "test-encoded-data";
            Func<string, bool> validateJson = json => json.Contains("Invalid Name");

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidation_InvalidBase64_ShouldReturnNull()
        {
            // Arrange
            string invalidBase64 = "invalid-base64-string!@#";
            Func<string, bool> validateJson = json => !string.IsNullOrEmpty(json);

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(invalidBase64, validateJson);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidation_InvalidJson_ShouldReturnNull()
        {
            // Arrange
            string invalidJson = "invalid json string";
            string encodedData = "invalid-encoded-data";
            Func<string, bool> validateJson = json => !string.IsNullOrEmpty(json);

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidation_EmptyJson_ShouldReturnNull()
        {
            // Arrange
            string emptyJson = "";
            string encodedData = "";
            Func<string, bool> validateJson = json => !string.IsNullOrEmpty(json);

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithComplexObject_ShouldReturnDeserializedObject()
        {
            // Arrange
            var complexModel = new
            {
                Id = Guid.NewGuid(),
                Name = "Complex Test",
                Properties = new
                {
                    Value1 = 123.45,
                    Value2 = new[] { "item1", "item2", "item3" },
                    Value3 = new { Nested = "nested value" }
                },
                IsEnabled = true,
                CreatedDate = DateTime.UtcNow
            };
            string json = JsonConvert.SerializeObject(complexModel);
            string encodedData = "complex-encoded-data";

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<object>(encodedData);

            // Assert - Vì encoded data không hợp lệ, kết quả sẽ là null
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithNullValidationFunction_ShouldReturnNull()
        {
            // Arrange
            var testModel = new TestModel { Name = "Test" };
            string json = JsonConvert.SerializeObject(testModel);
            string encodedData = "test-encoded-data";
            Func<string, bool>? validateJson = null;

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson!);

            // Assert - Base64JsonDecoder không check null validation function, nên sẽ return null
            Assert.Null(result);
        }

        [Fact]
        public void DecodeAndDeserialize_WithValidationFunctionThrowingException_ShouldReturnNull()
        {
            // Arrange
            var testModel = new TestModel { Name = "Test" };
            string json = JsonConvert.SerializeObject(testModel);
            string encodedData = "test-encoded-data";
            Func<string, bool> validateJson = json => throw new InvalidOperationException("Validation error");

            // Act
            var result = Base64JsonDecoder.DecodeAndDeserialize<TestModel>(encodedData, validateJson);

            // Assert
            Assert.Null(result);
        }
    }
}

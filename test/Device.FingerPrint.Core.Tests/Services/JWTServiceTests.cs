using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;

namespace Device.FingerPrint.Core.Tests.Services
{
    public class JWTServiceTests
    {
        private const string TestJwtPassword = "TestJWTSecretKey123!@#";
        private const string TestApiUrl = "https://test-api.example.com";
        private const string TestAudience = "test-audience";

        [Fact]
        public void Constructor_WithValidAppSettings_ShouldInitializeCorrectly()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();

            // Act
            var jwtService = new JWTService(mockAppSettings.Object);

            // Assert
            jwtService.Should().NotBeNull();
        }

        [Fact]
        public void GetJWTKey_ShouldReturnCloneOfKey()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);

            // Act
            var key1 = jwtService.GetJWTKey();
            var key2 = jwtService.GetJWTKey();

            // Assert
            key1.Should().NotBeNull();
            key2.Should().NotBeNull();
            key1.Should().BeEquivalentTo(key2);
            key1.Should().NotBeSameAs(key2); // Should be different instances (cloned)
        }

        [Fact]
        public void GetJWTKey_ShouldReturnCorrectKeyBytes()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expectedKey = Encoding.ASCII.GetBytes(TestJwtPassword);

            // Act
            var result = jwtService.GetJWTKey();

            // Assert
            result.Should().BeEquivalentTo(expectedKey);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);

            // Act
            var result = jwtService.GenerateJwtToken();

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$"); // JWT format
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnTokenWithCorrectClaims()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);

            // Act
            var token = jwtService.GenerateJwtToken();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.Claims.Should().ContainSingle(c => c.Type == "id" && c.Value == TestApiUrl);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnTokenWithCorrectExpiration()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var beforeGeneration = DateTimeOffset.UtcNow;

            // Act
            var token = jwtService.GenerateJwtToken();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().BeAfter(beforeGeneration.DateTime);
            jwtToken.ValidTo.Should().BeBefore(beforeGeneration.AddMinutes(2).DateTime); // Should expire within 2 minutes
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnTokenWithCorrectIssuer()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);

            // Act
            var token = jwtService.GenerateJwtToken();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.Issuer.Should().Be(TestApiUrl);
        }

        [Fact]
        public void GenerateJwtToken_ShouldReturnTokenWithCorrectSigningAlgorithm()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);

            // Act
            var token = jwtService.GenerateJwtToken();
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.SignatureAlgorithm.Should().Be("HS256");
        }

        [Fact]
        public void GenerateJWT_WithValidParameters_ShouldReturnValidJwtToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);

            // Act
            var result = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Should().MatchRegex("^[A-Za-z0-9-_=]+\\.[A-Za-z0-9-_=]+\\.?[A-Za-z0-9-_.+/=]*$"); // JWT format
        }

        [Fact]
        public void GenerateJWT_WithValidParameters_ShouldReturnTokenWithCorrectExpiration()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);

            // Act
            var token = jwtService.GenerateJWT(expiredTime, TestAudience);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().Be(expiredTime);
        }

        [Fact]
        public void GenerateJWT_WithValidParameters_ShouldReturnTokenWithCorrectSigningAlgorithm()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);

            // Act
            var token = jwtService.GenerateJWT(expiredTime, TestAudience);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.SignatureAlgorithm.Should().Be("HS256");
        }

        [Fact]
        public void GenerateJWT_WithValidParameters_ShouldUseCorrectSigningKey()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);
            var expectedKey = Encoding.UTF8.GetBytes(TestJwtPassword + "@" + TestAudience);

            // Act
            var token = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            token.Should().NotBeNull();
            // Verify the token can be validated with the expected key
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(expectedKey),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var validationResult = tokenHandler.ValidateToken(token, validationParameters, out _);
            validationResult.Identity.IsAuthenticated.Should().BeTrue();
        }

        [Fact]
        public void GenerateJWT_WithPastExpirationTime_ShouldReturnTokenWithPastExpiration()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(-1); // Past time

            // Act
            var token = jwtService.GenerateJWT(expiredTime, TestAudience);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().Be(expiredTime);
            jwtToken.ValidTo.Should().BeBefore(DateTime.UtcNow);
        }

        [Fact]
        public void GenerateJWT_WithFutureExpirationTime_ShouldReturnTokenWithFutureExpiration()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1); // Future time

            // Act
            var token = jwtService.GenerateJWT(expiredTime, TestAudience);
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            jwtToken.Should().NotBeNull();
            jwtToken.ValidTo.Should().Be(expiredTime);
            jwtToken.ValidTo.Should().BeAfter(DateTime.UtcNow);
        }

        [Fact]
        public void GenerateJWT_WithEmptyAudience_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);
            var emptyAudience = string.Empty;

            // Act
            var result = jwtService.GenerateJWT(expiredTime, emptyAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithNullAudience_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);
            string nullAudience = null;

            // Act
            var result = jwtService.GenerateJWT(expiredTime, nullAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithSpecialCharactersInAudience_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);
            var specialAudience = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

            // Act
            var result = jwtService.GenerateJWT(expiredTime, specialAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithUnicodeCharactersInAudience_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);
            var unicodeAudience = "Unicode test: 你好世界 🌍 🚀";

            // Act
            var result = jwtService.GenerateJWT(expiredTime, unicodeAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithLongAudience_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);
            var longAudience = new string('A', 1000); // 1000 character audience

            // Act
            var result = jwtService.GenerateJWT(expiredTime, longAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithMinDateTime_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.MinValue;

            // Act
            var result = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithMaxDateTime_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.MaxValue;

            // Act
            var result = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithUtcNow_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow;

            // Act
            var result = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithDifferentAudiences_ShouldReturnDifferentTokens()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);
            var audience1 = "audience1";
            var audience2 = "audience2";

            // Act
            var token1 = jwtService.GenerateJWT(expiredTime, audience1);
            var token2 = jwtService.GenerateJWT(expiredTime, audience2);

            // Assert
            token1.Should().NotBe(token2);
            token1.Should().NotBeNull();
            token2.Should().NotBeNull();
        }

        [Fact]
        public void GenerateJWT_WithSameParameters_ShouldReturnDifferentTokensDueToTime()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddHours(1);

            // Act
            var token1 = jwtService.GenerateJWT(expiredTime, TestAudience);
            var token2 = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            token1.Should().NotBe(token2); // Different due to different generation times
            token1.Should().NotBeNull();
            token2.Should().NotBeNull();
        }

        [Fact]
        public void GenerateJWT_WithVeryShortExpiration_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddMilliseconds(100); // 100ms

            // Act
            var result = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        [Fact]
        public void GenerateJWT_WithVeryLongExpiration_ShouldReturnValidToken()
        {
            // Arrange
            var mockAppSettings = CreateMockAppSettings();
            var jwtService = new JWTService(mockAppSettings.Object);
            var expiredTime = DateTime.UtcNow.AddYears(100); // 100 years

            // Act
            var result = jwtService.GenerateJWT(expiredTime, TestAudience);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
        }

        #region Helper Methods

        private static Mock<IAppSettings> CreateMockAppSettings()
        {
            var mock = new Mock<IAppSettings>();
            mock.Setup(x => x.JWTPassword).Returns(TestJwtPassword);
            mock.Setup(x => x.ApiUrl).Returns(TestApiUrl);
            return mock;
        }

        #endregion
    }
}

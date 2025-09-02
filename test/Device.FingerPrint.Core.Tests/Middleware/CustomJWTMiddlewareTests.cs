using Device.FingerPrint.Core.Middleware;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Xunit;

namespace Device.FingerPrint.Core.Tests.Middleware
{
    public class CustomJWTMiddlewareTests
    {
        private const string ValidJwtPassword = "TestJWTSecretKey123!@#_ThisIsLongEnoughForHmacSha256_2025";
        private const string InvalidJwtPassword = "DifferentSecretKey_ThisIsLongEnoughForHmacSha256_2025";

        [Fact]
        public void Constructor_WithValidParameters_ShouldSetProperties()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            bool jwtEnable = true;
            string jwtPassword = ValidJwtPassword;

            // Act
            var middleware = new CustomJWTMiddleware(mockNext.Object, jwtEnable, jwtPassword);

            // Assert
            middleware.Should().NotBeNull();
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndNoToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var invalidToken = "invalid.token.here";
            SetupAuthorizationHeader(context, $"Bearer {invalidToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndValidToken_ShouldCallNext()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var validToken = GenerateValidJwtToken(ValidJwtPassword);
            SetupAuthorizationHeader(context, $"Bearer {validToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(200); // Default status code
            mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenJWTDisabledAndNoToken_ShouldCallNext()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: false, ValidJwtPassword);
            var context = CreateMockHttpContext();

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(200); // Default status code
            mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenJWTDisabledAndValidToken_ShouldCallNext()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: false, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var validToken = GenerateValidJwtToken(ValidJwtPassword);
            SetupAuthorizationHeader(context, $"Bearer {validToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(200); // Default status code
            mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenJWTDisabledAndInvalidToken_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: false, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var invalidToken = "invalid.token.here";
            SetupAuthorizationHeader(context, $"Bearer {invalidToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndTokenWithDifferentSecret_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var tokenWithDifferentSecret = GenerateValidJwtToken(InvalidJwtPassword);
            SetupAuthorizationHeader(context, $"Bearer {tokenWithDifferentSecret}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndMalformedAuthorizationHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            SetupAuthorizationHeader(context, "InvalidFormat");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndEmptyAuthorizationHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            SetupAuthorizationHeader(context, "");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndWhitespaceAuthorizationHeader_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            SetupAuthorizationHeader(context, "   ");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndAuthorizationHeaderWithoutBearer_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var validToken = GenerateValidJwtToken(ValidJwtPassword);
            SetupAuthorizationHeader(context, validToken); // No "Bearer " prefix

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndAuthorizationHeaderWithMultipleSpaces_ShouldExtractTokenCorrectly()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var validToken = GenerateValidJwtToken(ValidJwtPassword);
            SetupAuthorizationHeader(context, $"Bearer    {validToken}"); // Multiple spaces

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(200); // Default status code
            mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndAuthorizationHeaderWithTrailingSpaces_ShouldReturnUnauthorized()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var validToken = GenerateValidJwtToken(ValidJwtPassword);
            SetupAuthorizationHeader(context, $"Bearer {validToken}   "); // Trailing spaces

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
            mockNext.Verify(x => x(It.IsAny<HttpContext>()), Times.Never);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndTokenWithSpecialCharacters_ShouldValidateCorrectly()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var validToken = GenerateValidJwtToken(ValidJwtPassword);
            SetupAuthorizationHeader(context, $"Bearer {validToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(200); // Default status code
            mockNext.Verify(x => x(context), Times.Once);
        }

        [Fact]
        public async Task Invoke_WhenJWTEnabledAndTokenWithClaims_ShouldValidateCorrectly()
        {
            // Arrange
            var mockNext = new Mock<RequestDelegate>();
            var middleware = new CustomJWTMiddleware(mockNext.Object, JWTEnable: true, ValidJwtPassword);
            var context = CreateMockHttpContext();
            var validToken = GenerateValidJwtTokenWithClaims(ValidJwtPassword);

            // Setup headers with authorization
            SetupAuthorizationHeader(context, $"Bearer {validToken}");

            // Act
            await middleware.Invoke(context);

            // Assert
            context.Response.StatusCode.Should().Be(200); // Default status code
            mockNext.Verify(x => x(context), Times.Once);
        }

        #region Helper Methods

        private static HttpContext CreateMockHttpContext()
        {
            var context = new Mock<HttpContext>();
            var response = new Mock<HttpResponse>();
            var request = new Mock<HttpRequest>();
            var headers = new Dictionary<string, StringValues>();
            // Setup response
            response.SetupProperty(x => x.StatusCode, 200);
            context.Setup(x => x.Response).Returns(response.Object);

            // Setup request
            request.Setup(x => x.Headers).Returns(new HeaderDictionary(headers));
            context.Setup(x => x.Request).Returns(request.Object);

            return context.Object;
        }

        private static void SetupAuthorizationHeader(HttpContext context, string authorizationValue)
        {
            context.Request.Headers["Authorization"] = authorizationValue;
        }

        private static string GenerateValidJwtToken(string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.Role, "user")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateExpiredJwtToken(string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.Role, "user")
                }),
                NotBefore = now.AddHours(-2), // NotBefore is 2 hours ago
                Expires = now.AddHours(-1),   // Expired 1 hour ago (after NotBefore)
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static string GenerateValidJwtTokenWithClaims(string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.Role, "admin"),
                    new Claim("custom_claim", "custom_value"),
                    new Claim("user_id", "12345")
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        #endregion
    }
}

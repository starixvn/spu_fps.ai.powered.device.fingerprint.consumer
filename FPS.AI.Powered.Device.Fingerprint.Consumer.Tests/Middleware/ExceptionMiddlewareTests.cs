using Device.FingerPrint.QueueConsumer;
using Fanex.Logging;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using FluentAssertions.Extensions;
using Xunit;
using System.Threading.Tasks;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Tests.Middleware
{
    public class ExceptionMiddlewareTests
    {
        private readonly Fanex.Logging.ILogger _mockLogger;

        public ExceptionMiddlewareTests()
        {
            _mockLogger = Substitute.For<Fanex.Logging.ILogger>();
        }

        [Fact]
        public void Constructor_WhenCalled_ShouldCreateMiddlewareInstance()
        {
            // Arrange
            RequestDelegate next = context => Task.CompletedTask;

            // Act
            var middleware = new ExceptionMiddleware(next, _mockLogger);

            // Assert
            middleware.Should().NotBeNull();
        }

        [Fact]
        public async Task InvokeAsync_WhenNoException_ShouldCallNextMiddleware()
        {
            // Arrange
            var wasNextCalled = false;
            RequestDelegate next = context =>
            {
                wasNextCalled = true;
                return Task.CompletedTask;
            };

            var middleware = new ExceptionMiddleware(next, _mockLogger);
            var httpContext = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            wasNextCalled.Should().BeTrue();
            _mockLogger.DidNotReceive().ErrorAsync(Arg.Any<string>());
        }

        [Fact]
        public async Task InvokeAsync_WhenExceptionOccurs_ShouldLogError()
        {
            // Arrange
            var exception = new Exception("Test exception");
            RequestDelegate next = context => throw exception;

            var middleware = new ExceptionMiddleware(next, _mockLogger);
            var httpContext = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            _mockLogger.Received(1).ErrorAsync(Arg.Is<string>(s => s.Contains("Test exception")));
        }

        [Fact]
        public async Task InvokeAsync_WhenExceptionOccurs_ShouldNotPropagateException()
        {
            // Arrange
            var exception = new Exception("Test exception");
            RequestDelegate next = context => throw exception;

            var middleware = new ExceptionMiddleware(next, _mockLogger);
            var httpContext = new DefaultHttpContext();

            // Act & Assert
            await middleware.InvokeAsync(httpContext);
            // Should not throw exception
        }

        [Fact]
        public async Task InvokeAsync_WhenCustomExceptionOccurs_ShouldLogErrorWithExceptionDetails()
        {
            // Arrange
            var customException = new InvalidOperationException("Custom operation failed");
            RequestDelegate next = context => throw customException;

            var middleware = new ExceptionMiddleware(next, _mockLogger);
            var httpContext = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(httpContext);

            // Assert
            _mockLogger.Received(1).ErrorAsync(Arg.Is<string>(s => 
                s.Contains("Custom operation failed")));
        }

        [Fact]
        public async Task InvokeAsync_WhenMultipleExceptionsOccur_ShouldLogEachException()
        {
            // Arrange
            var exception1 = new Exception("First exception");
            var exception2 = new Exception("Second exception");

            var callCount = 0;
            RequestDelegate next = context =>
            {
                callCount++;
                if (callCount == 1)
                    throw exception1;
                else
                    throw exception2;
            };

            var middleware = new ExceptionMiddleware(next, _mockLogger);
            var httpContext = new DefaultHttpContext();

            // Act
            await middleware.InvokeAsync(httpContext);
            await middleware.InvokeAsync(httpContext);

            // Assert
            _mockLogger.Received(2).ErrorAsync(Arg.Any<string>());
            _mockLogger.Received(1).ErrorAsync(Arg.Is<string>(s => s.Contains("First exception")));
            _mockLogger.Received(1).ErrorAsync(Arg.Is<string>(s => s.Contains("Second exception")));
        }

        [Fact]
        public async Task InvokeAsync_WhenNullHttpContext_ShouldHandleGracefully()
        {
            // Arrange
            RequestDelegate next = context => Task.CompletedTask;
            var middleware = new ExceptionMiddleware(next, _mockLogger);

            // Act & Assert
            await middleware.InvokeAsync(null!);
            // Should not throw exception
        }


    }
}

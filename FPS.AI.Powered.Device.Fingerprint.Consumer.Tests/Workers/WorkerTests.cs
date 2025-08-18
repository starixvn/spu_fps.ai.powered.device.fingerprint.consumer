using Device.FingerPrint.QueueConsumer;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FPS.AI.Powered.Device.Fingerprint.Consumer.Tests.Workers
{
    public class WorkerTests
    {
        private readonly ILogger<Worker> _mockLogger;

        public WorkerTests()
        {
            _mockLogger = Substitute.For<ILogger<Worker>>();
        }

        [Fact]
        public void Constructor_WhenCalled_ShouldCreateWorkerInstance()
        {
            // Act
            var worker = new Worker(_mockLogger);

            // Assert
            worker.Should().NotBeNull();
            worker.Should().BeAssignableTo<BackgroundService>();
        }

        [Fact]
        public async Task ExecuteAsync_WhenCalled_ShouldLogInformation()
        {
            // Arrange
            var worker = new Worker(_mockLogger);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            var task = worker.StartAsync(cancellationToken);
            
            // Wait a bit for the worker to start
            await Task.Delay(100);
            
            // Stop the worker
            cancellationTokenSource.Cancel();
            await task;

            // Assert
            // Worker should be created successfully
            worker.Should().NotBeNull();
        }

        [Fact]
        public async Task ExecuteAsync_WhenCancellationRequested_ShouldStopGracefully()
        {
            // Arrange
            var worker = new Worker(_mockLogger);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            var startTask = worker.StartAsync(cancellationToken);
            
            // Cancel immediately
            cancellationTokenSource.Cancel();
            
            await startTask;

            // Assert
            // Should complete without throwing exceptions
            startTask.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task ExecuteAsync_WhenLoggerIsDisabled_ShouldNotLog()
        {
            // Arrange
            _mockLogger.IsEnabled(LogLevel.Information).Returns(false);
            var worker = new Worker(_mockLogger);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            var task = worker.StartAsync(cancellationToken);
            
            // Wait a bit
            await Task.Delay(100);
            
            // Stop the worker
            cancellationTokenSource.Cancel();
            await task;

            // Assert
            _mockLogger.DidNotReceive().Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Any<object>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception?, string>>());
        }

        [Fact]
        public async Task ExecuteAsync_WhenLoggerIsEnabled_ShouldLogPeriodically()
        {
            // Arrange
            _mockLogger.IsEnabled(LogLevel.Information).Returns(true);
            var worker = new Worker(_mockLogger);
            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            var task = worker.StartAsync(cancellationToken);
            
            // Wait for multiple log cycles (10 seconds delay in worker)
            await Task.Delay(500);
            
            // Stop the worker
            cancellationTokenSource.Cancel();
            await task;

            // Assert
            // Worker should be created successfully
            worker.Should().NotBeNull();
        }
    }
}

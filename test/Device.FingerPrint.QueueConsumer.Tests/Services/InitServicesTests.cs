using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Device.FingerPrint.QueueConsumer.Tests.Services
{
    public class InitServicesTests
    {
        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterAllServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify Transaction Services
            serviceProvider.GetService<ITransactionDataAccess>().Should().NotBeNull();
            serviceProvider.GetService<ITransactionService>().Should().NotBeNull();

            // Verify Bot Detection Service
            serviceProvider.GetService<IBotDetectionService>().Should().NotBeNull();
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterServicesAsSingletons()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Get first instance
            var firstTransactionService = serviceProvider.GetService<ITransactionService>();
            var firstBotDetectionService = serviceProvider.GetService<IBotDetectionService>();

            // Get second instance
            var secondTransactionService = serviceProvider.GetService<ITransactionService>();
            var secondBotDetectionService = serviceProvider.GetService<IBotDetectionService>();

            // Verify they are the same instance (singleton)
            firstTransactionService.Should().BeSameAs(secondTransactionService);
            firstBotDetectionService.Should().BeSameAs(secondBotDetectionService);
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterAllRequiredInterfaces()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify all interfaces are registered and can be resolved
            serviceProvider.GetRequiredService<ITransactionDataAccess>().Should().NotBeNull();
            serviceProvider.GetRequiredService<ITransactionService>().Should().NotBeNull();
            serviceProvider.GetRequiredService<IBotDetectionService>().Should().NotBeNull();
        }

        [Fact]
        public void RegisterService_WhenCalledMultipleTimes_ShouldNotThrowException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            services.RegisterService();
            services.RegisterService(); // Should not throw

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetService<ITransactionService>().Should().NotBeNull();
        }

        [Fact]
        public void RegisterService_WhenCalled_ShouldRegisterServicesInCorrectOrder()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify services can be resolved without dependency issues
            var transactionService = serviceProvider.GetService<ITransactionService>();
            var botDetectionService = serviceProvider.GetService<IBotDetectionService>();

            // All services should be successfully resolved
            transactionService.Should().NotBeNull();
            botDetectionService.Should().NotBeNull();
        }
    }
}

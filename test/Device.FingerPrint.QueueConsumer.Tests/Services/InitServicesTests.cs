using Device.FingerPrint.Features.LogInfo.Services;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Queue;
using Device.FingerPrint.QueueConsumer.Handlers;
using Device.FingerPrint.QueueConsumer.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

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

            // Verify Device Service
            serviceProvider.GetService<IDeviceService>().Should().NotBeNull();

            // Verify MassTransit Services
            serviceProvider.GetService<IMassTransitProducer>().Should().NotBeNull();
            serviceProvider.GetService<Consumer>().Should().NotBeNull();

            // Verify Bot Detection Service
            serviceProvider.GetService<IBotDetectionService>().Should().NotBeNull();

            // Verify Log Info Services
            serviceProvider.GetService<ILogInfoDataAccess>().Should().NotBeNull();
            serviceProvider.GetService<ILogInfoServices>().Should().NotBeNull();
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
            var firstDeviceService = serviceProvider.GetService<IDeviceService>();
            var firstBotDetectionService = serviceProvider.GetService<IBotDetectionService>();
            var firstLogInfoService = serviceProvider.GetService<ILogInfoServices>();
            var firstMassTransitProducer = serviceProvider.GetService<IMassTransitProducer>();
            var firstConsumer = serviceProvider.GetService<Consumer>();

            // Get second instance
            var secondTransactionService = serviceProvider.GetService<ITransactionService>();
            var secondDeviceService = serviceProvider.GetService<IDeviceService>();
            var secondBotDetectionService = serviceProvider.GetService<IBotDetectionService>();
            var secondLogInfoService = serviceProvider.GetService<ILogInfoServices>();
            var secondMassTransitProducer = serviceProvider.GetService<IMassTransitProducer>();
            var secondConsumer = serviceProvider.GetService<Consumer>();

            // Verify they are the same instance (singleton)
            firstTransactionService.Should().BeSameAs(secondTransactionService);
            firstDeviceService.Should().BeSameAs(secondDeviceService);
            firstBotDetectionService.Should().BeSameAs(secondBotDetectionService);
            firstLogInfoService.Should().BeSameAs(secondLogInfoService);
            firstMassTransitProducer.Should().BeSameAs(secondMassTransitProducer);
            firstConsumer.Should().BeSameAs(secondConsumer);
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
            serviceProvider.GetRequiredService<IDeviceService>().Should().NotBeNull();
            serviceProvider.GetRequiredService<IMassTransitProducer>().Should().NotBeNull();
            serviceProvider.GetRequiredService<Consumer>().Should().NotBeNull();
            serviceProvider.GetRequiredService<IBotDetectionService>().Should().NotBeNull();
            serviceProvider.GetRequiredService<ILogInfoDataAccess>().Should().NotBeNull();
            serviceProvider.GetRequiredService<ILogInfoServices>().Should().NotBeNull();
        }

        [Fact]
        public void RegisterService_WhenCalledMultipleTimes_ShouldNotThrowException()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert
            services.RegisterService();
            services.RegisterService(); // Should not throw exception

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
            var deviceService = serviceProvider.GetService<IDeviceService>();
            var botDetectionService = serviceProvider.GetService<IBotDetectionService>();
            var logInfoService = serviceProvider.GetService<ILogInfoServices>();
            var massTransitProducer = serviceProvider.GetService<IMassTransitProducer>();
            var consumer = serviceProvider.GetService<Consumer>();

            // All services should be successfully resolved
            transactionService.Should().NotBeNull();
            deviceService.Should().NotBeNull();
            botDetectionService.Should().NotBeNull();
            logInfoService.Should().NotBeNull();
            massTransitProducer.Should().NotBeNull();
            consumer.Should().NotBeNull();
        }

        [Fact]
        public void RegisterService_ShouldRegisterTransactionServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify Transaction-related services
            var transactionDataAccess = serviceProvider.GetService<ITransactionDataAccess>();
            var transactionService = serviceProvider.GetService<ITransactionService>();

            transactionDataAccess.Should().NotBeNull();
            transactionService.Should().NotBeNull();

            // Verify they are singletons
            var secondTransactionDataAccess = serviceProvider.GetService<ITransactionDataAccess>();
            var secondTransactionService = serviceProvider.GetService<ITransactionService>();

            transactionDataAccess.Should().BeSameAs(secondTransactionDataAccess);
            transactionService.Should().BeSameAs(secondTransactionService);
        }

        [Fact]
        public void RegisterService_ShouldRegisterDeviceService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify Device service
            var deviceService = serviceProvider.GetService<IDeviceService>();
            deviceService.Should().NotBeNull();

            // Verify it's a singleton
            var secondDeviceService = serviceProvider.GetService<IDeviceService>();
            deviceService.Should().BeSameAs(secondDeviceService);
        }

        [Fact]
        public void RegisterService_ShouldRegisterMassTransitServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify MassTransit-related services
            var massTransitProducer = serviceProvider.GetService<IMassTransitProducer>();
            var consumer = serviceProvider.GetService<Consumer>();

            massTransitProducer.Should().NotBeNull();
            consumer.Should().NotBeNull();

            // Verify they are singletons
            var secondMassTransitProducer = serviceProvider.GetService<IMassTransitProducer>();
            var secondConsumer = serviceProvider.GetService<Consumer>();

            massTransitProducer.Should().BeSameAs(secondMassTransitProducer);
            consumer.Should().BeSameAs(secondConsumer);
        }

        [Fact]
        public void RegisterService_ShouldRegisterBotDetectionService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify Bot Detection service
            var botDetectionService = serviceProvider.GetService<IBotDetectionService>();
            botDetectionService.Should().NotBeNull();

            // Verify it's a singleton
            var secondBotDetectionService = serviceProvider.GetService<IBotDetectionService>();
            botDetectionService.Should().BeSameAs(secondBotDetectionService);
        }

        [Fact]
        public void RegisterService_ShouldRegisterLogInfoServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify Log Info services
            var logInfoDataAccess = serviceProvider.GetService<ILogInfoDataAccess>();
            var logInfoServices = serviceProvider.GetService<ILogInfoServices>();

            logInfoDataAccess.Should().NotBeNull();
            logInfoServices.Should().NotBeNull();

            // Verify they are singletons
            var secondLogInfoDataAccess = serviceProvider.GetService<ILogInfoDataAccess>();
            var secondLogInfoServices = serviceProvider.GetService<ILogInfoServices>();

            logInfoDataAccess.Should().BeSameAs(secondLogInfoDataAccess);
            logInfoServices.Should().BeSameAs(secondLogInfoServices);
        }

        [Fact]
        public void RegisterService_WithEmptyServiceCollection_ShouldRegisterAllServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var initialCount = services.Count;

            // Act
            services.RegisterService();

            // Assert
            services.Count.Should().BeGreaterThan(initialCount);
            
            var serviceProvider = services.BuildServiceProvider();
            
            // Should be able to resolve all main services
            serviceProvider.GetService<ITransactionService>().Should().NotBeNull();
            serviceProvider.GetService<IDeviceService>().Should().NotBeNull();
            serviceProvider.GetService<IBotDetectionService>().Should().NotBeNull();
            serviceProvider.GetService<ILogInfoServices>().Should().NotBeNull();
            serviceProvider.GetService<IMassTransitProducer>().Should().NotBeNull();
            serviceProvider.GetService<Consumer>().Should().NotBeNull();
        }

        [Fact]
        public void RegisterService_ShouldRegisterCorrectImplementations()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Verify correct implementations are registered
            var transactionService = serviceProvider.GetService<ITransactionService>();
            var deviceService = serviceProvider.GetService<IDeviceService>();
            var botDetectionService = serviceProvider.GetService<IBotDetectionService>();
            var logInfoService = serviceProvider.GetService<ILogInfoServices>();

            transactionService.Should().BeOfType<TransactionService>();
            deviceService.Should().BeOfType<DeviceService>();
            botDetectionService.Should().BeOfType<BotDetectionService>();
            logInfoService.Should().BeOfType<LogInfoServices>();
        }

        [Fact]
        public void RegisterService_ShouldAllowServiceResolutionWithoutExceptions()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterService();

            // Assert
            var serviceProvider = services.BuildServiceProvider();

            // Should not throw when resolving services
            Action resolveAllServices = () =>
            {
                serviceProvider.GetRequiredService<ITransactionDataAccess>();
                serviceProvider.GetRequiredService<ITransactionService>();
                serviceProvider.GetRequiredService<IDeviceService>();
                serviceProvider.GetRequiredService<IMassTransitProducer>();
                serviceProvider.GetRequiredService<Consumer>();
                serviceProvider.GetRequiredService<IBotDetectionService>();
                serviceProvider.GetRequiredService<ILogInfoDataAccess>();
                serviceProvider.GetRequiredService<ILogInfoServices>();
            };

            resolveAllServices.Should().NotThrow();
        }
    }
}
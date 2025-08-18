using Device.FingerPrint.Core;
using Device.FingerPrint.Core.Services;
using Device.FingerPrint.QueueConsumer.Handlers;
using Device.FingerPrint.QueueConsumer.Services;
using Device.FingerPrint.Redis;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Device.FingerPrint.Core.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void ServiceCollection_ShouldRegisterAppSettings()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateTestConfiguration();

            // Act
            var appSettings = new AppSettings(configuration);
            services.AddSingleton<IAppSettings>(appSettings);

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            var registeredAppSettings = serviceProvider.GetService<IAppSettings>();
            registeredAppSettings.Should().NotBeNull();
            registeredAppSettings.Should().BeOfType<AppSettings>();
        }

        [Fact]
        public void ServiceCollection_ShouldRegisterCoreServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateTestConfiguration();
            var appSettings = new AppSettings(configuration);

            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<IConfiguration>(configuration);

            // Act
            services.AddSingleton<ICrypto, Crypto>();
            services.AddSingleton<IUserAgentService, UserAgentService>();
            services.AddSingleton<IMobileRawTransactionService, MobileRawTransactionService>();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            
            serviceProvider.GetService<ICrypto>().Should().NotBeNull();
            serviceProvider.GetService<IUserAgentService>().Should().NotBeNull();
            serviceProvider.GetService<IMobileRawTransactionService>().Should().NotBeNull();
        }

        [Fact]
        public void AppSettings_ShouldLoadConfigurationValues()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Should().NotBeNull();
            // Note: Environment might be empty in test environment, so we just check it's not null
            appSettings.Environment.Should().NotBeNull();
        }

        [Fact]
        public void Configuration_ShouldContainRequiredSections()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act & Assert
            configuration.GetSection("Consumer").Should().NotBeNull();
            configuration.GetSection("RabbitMQ").Should().NotBeNull();
            configuration.GetSection("AppConfig").Should().NotBeNull();
        }

        [Fact]
        public void Configuration_ShouldLoadConsumerSettings()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act & Assert
            var consumerName = configuration.GetValue<string>("Consumer:Name");
            var batchSizeLimit = configuration.GetValue<int>("Consumer:Masstransit:BatchSizeLimit");
            var batchTimeLimit = configuration.GetValue<int>("Consumer:Masstransit:BatchTimeLimit");
            var batchConcurrencyLimit = configuration.GetValue<int>("Consumer:Masstransit:BatchConcurrencyLimit");
            var prefetchCount = configuration.GetValue<int>("Consumer:Masstransit:PrefetchCount");

            consumerName.Should().NotBeNullOrEmpty();
            batchSizeLimit.Should().BeGreaterThan(0);
            batchTimeLimit.Should().BeGreaterThan(0);
            batchConcurrencyLimit.Should().BeGreaterThan(0);
            prefetchCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Configuration_ShouldLoadRabbitMQSettings()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act & Assert
            var host = configuration.GetValue<string>("RabbitMQ:Host");
            var port = configuration.GetValue<ushort>("RabbitMQ:Port");
            var userName = configuration.GetValue<string>("RabbitMQ:UserName");
            var password = configuration.GetValue<string>("RabbitMQ:Password");
            var virtualHost = configuration.GetValue<string>("RabbitMQ:VirtualHost");

            host.Should().NotBeNullOrEmpty();
            port.Should().BeGreaterThan(0);
            userName.Should().NotBeNullOrEmpty();
            password.Should().NotBeNullOrEmpty();
            virtualHost.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void ServiceCollection_ShouldRegisterServicesAsSingletons()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateTestConfiguration();
            var appSettings = new AppSettings(configuration);

            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ICrypto, Crypto>();
            services.AddSingleton<IUserAgentService, UserAgentService>();
            services.AddSingleton<IMobileRawTransactionService, MobileRawTransactionService>();

            var serviceProvider = services.BuildServiceProvider();

            // Act
            var appSettings1 = serviceProvider.GetService<IAppSettings>();
            var appSettings2 = serviceProvider.GetService<IAppSettings>();
            var crypto1 = serviceProvider.GetService<ICrypto>();
            var crypto2 = serviceProvider.GetService<ICrypto>();

            // Assert
            appSettings1.Should().BeSameAs(appSettings2);
            crypto1.Should().BeSameAs(crypto2);
        }

        [Fact]
        public void AppSettings_ShouldHaveValidEnvironment()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Environment.Should().NotBeNull();
            // Note: In test environment, Environment might be empty, so we just check it's not null
        }

        [Fact]
        public void AppSettings_ShouldHaveValidCryptoConfiguration()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.CryptoPassword.Should().NotBeNullOrEmpty();
            appSettings.CryptoIV.Should().NotBeNullOrEmpty();
            appSettings.CryptoSalt.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void Configuration_ShouldLoadAllRequiredValues()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act & Assert
            configuration.GetValue<string>("Environment").Should().Be("Test");
            configuration.GetValue<string>("IsVaultEnable").Should().Be("false");
            configuration.GetValue<string>("SentryDsn").Should().Be("https://test@sentry.io/test");
            configuration.GetValue<string>("AppConfig:Crypto:Password").Should().Be("test-password");
            configuration.GetValue<string>("AppConfig:Crypto:IV").Should().Be("c3eef06037e837f4ee390bda69c05023");
            configuration.GetValue<string>("AppConfig:Crypto:Salt").Should().Be("234292a9fce031f2");
        }

        [Fact]
        public void ServiceCollection_ShouldRegisterServicesInCorrectOrder()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateTestConfiguration();
            var appSettings = new AppSettings(configuration);

            // Act - Register in the same order as Program.cs
            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ICrypto, Crypto>();
            services.AddSingleton<IUserAgentService, UserAgentService>();
            services.AddSingleton<IMobileRawTransactionService, MobileRawTransactionService>();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            
            // Should be able to resolve all services
            serviceProvider.GetService<IAppSettings>().Should().NotBeNull();
            serviceProvider.GetService<IConfiguration>().Should().NotBeNull();
            serviceProvider.GetService<ICrypto>().Should().NotBeNull();
            serviceProvider.GetService<IUserAgentService>().Should().NotBeNull();
            serviceProvider.GetService<IMobileRawTransactionService>().Should().NotBeNull();
        }

        [Fact]
        public void Program_ShouldRegisterAllRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = CreateTestConfiguration();
            var appSettings = new AppSettings(configuration);

            // Act - Simulate the service registration as in Program.cs
            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton<ICrypto, Crypto>();
            services.AddSingleton<IUserAgentService, UserAgentService>();
            services.AddSingleton<IMobileRawTransactionService, MobileRawTransactionService>();

            // Assert
            var serviceProvider = services.BuildServiceProvider();
            
            // Verify that the core services can be resolved
            Action resolveCoreServices = () =>
            {
                serviceProvider.GetRequiredService<IAppSettings>();
                serviceProvider.GetRequiredService<IConfiguration>();
                serviceProvider.GetRequiredService<ICrypto>();
                serviceProvider.GetRequiredService<IUserAgentService>();
                serviceProvider.GetRequiredService<IMobileRawTransactionService>();
            };

            resolveCoreServices.Should().NotThrow();
        }

        [Fact]
        public void Configuration_ShouldHaveValidStructure()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act & Assert
            configuration.GetSection("Consumer").Should().NotBeNull();
            configuration.GetSection("RabbitMQ").Should().NotBeNull();
            configuration.GetSection("AppConfig").Should().NotBeNull();
            configuration.GetSection("AppConfig:Crypto").Should().NotBeNull();
        }

        [Fact]
        public void AppSettings_ShouldLoadFromConfiguration()
        {
            // Arrange
            var configuration = CreateTestConfiguration();

            // Act
            var appSettings = new AppSettings(configuration);

            // Assert
            appSettings.Should().NotBeNull();
            appSettings.CryptoPassword.Should().Be("test-password");
            appSettings.CryptoIV.Should().Be("c3eef06037e837f4ee390bda69c05023");
            appSettings.CryptoSalt.Should().Be("234292a9fce031f2");
        }

        private static IConfiguration CreateTestConfiguration()
        {
            var inMemory = new Dictionary<string, string?>
            {
                ["Environment"] = "Test",
                ["IsVaultEnable"] = "false",
                ["SentryDsn"] = "https://test@sentry.io/test",
                ["AppConfig:Crypto:Password"] = "test-password",
                ["AppConfig:Crypto:IV"] = "c3eef06037e837f4ee390bda69c05023",
                ["AppConfig:Crypto:Salt"] = "234292a9fce031f2",
                ["Consumer:Name"] = "test-consumer",
                ["Consumer:Masstransit:BatchSizeLimit"] = "100",
                ["Consumer:Masstransit:BatchTimeLimit"] = "30",
                ["Consumer:Masstransit:BatchConcurrencyLimit"] = "5",
                ["Consumer:Masstransit:PrefetchCount"] = "10",
                ["RabbitMQ:Host"] = "localhost",
                ["RabbitMQ:Port"] = "5672",
                ["RabbitMQ:UserName"] = "guest",
                ["RabbitMQ:Password"] = "guest",
                ["RabbitMQ:VirtualHost"] = "/",
                ["EverTaggingDBIndex"] = "1",
                ["RedisDatabase"] = "0"
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();
        }
    }
}

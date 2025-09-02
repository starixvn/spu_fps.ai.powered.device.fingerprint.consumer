using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Device.FingerPrint.QueueConsumer;
using Xunit;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Device.FingerPrint.QueueConsumer.Tests
{
    public class ProgramTests
    {
        [Fact]
        public void CreateHostBuilder_Should_Return_Valid_HostBuilder()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            Assert.NotNull(hostBuilder);
            Assert.IsAssignableFrom<IHostBuilder>(hostBuilder);
        }

        [Fact]
        public void CreateHostBuilder_Should_Accept_Arguments()
        {
            // Arrange
            string[] args = { "--environment", "Testing", "--urls", "http://localhost:5000" };

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            Assert.NotNull(hostBuilder);
        }

        [Fact]
        public void CreateHostBuilder_Should_Configure_Default_Services()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);
            var host = hostBuilder.Build();

            // Assert
            Assert.NotNull(host);
            Assert.NotNull(host.Services);
            
            // Verify basic services are registered
            var configuration = host.Services.GetService(typeof(IConfiguration));
            Assert.NotNull(configuration);
        }

        [Fact]
        public void CreateHostBuilder_Should_Use_Startup_Class()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);
            var host = hostBuilder.Build();

            // Assert
            Assert.NotNull(host);
            
            // Verify that web host is configured
            var webHost = host.Services.GetService<IWebHostEnvironment>();
            Assert.NotNull(webHost);
        }

        [Fact]
        public void CreateHostBuilder_Should_Configure_Environment_Variables()
        {
            // Arrange
            string[] args = Array.Empty<string>();
            var testEnvKey = "TEST_ENV_VARIABLE";
            var testEnvValue = "TestEnvironmentValue";

            try
            {
                Environment.SetEnvironmentVariable(testEnvKey, testEnvValue);

                // Act
                var hostBuilder = Program.CreateHostBuilder(args);
                var host = hostBuilder.Build();
                var configuration = host.Services.GetRequiredService<IConfiguration>();

                // Assert
                Assert.NotNull(configuration);
                Assert.Equal(testEnvValue, configuration[testEnvKey]);
            }
            finally
            {
                // Cleanup
                Environment.SetEnvironmentVariable(testEnvKey, null);
            }
        }

        [Fact]
        public void CreateHostBuilder_Should_Handle_Empty_Args()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act & Assert
            var exception = Record.Exception(() => Program.CreateHostBuilder(args));
            Assert.Null(exception);
        }

        [Fact]
        public void CreateHostBuilder_Should_Handle_Null_Args()
        {
            // Arrange
            string[]? args = null;

            // Act & Assert
            var exception = Record.Exception(() => Program.CreateHostBuilder(args!));
            Assert.Null(exception);
        }

        [Fact]
        public void CreateHostBuilder_Should_Configure_WebHost_With_Startup()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);
            
            // Build host to verify configuration
            var host = hostBuilder.Build();

            // Assert
            Assert.NotNull(host);
            
            // Verify web host environment is configured
            var webHostEnvironment = host.Services.GetService<IWebHostEnvironment>();
            Assert.NotNull(webHostEnvironment);
        }

        [Fact]
        public async Task CreateHostBuilder_Built_Host_Should_Be_Disposable()
        {
            // Arrange
            string[] args = Array.Empty<string>();

            // Act
            var hostBuilder = Program.CreateHostBuilder(args);
            var host = hostBuilder.Build();

            // Assert
            Assert.NotNull(host);
            Assert.IsAssignableFrom<IDisposable>(host);

            // Should dispose without throwing
            var exception = await Record.ExceptionAsync(async () =>
            {
                await host.StopAsync();
                host.Dispose();
            });
            
            Assert.Null(exception);
        }

        [Theory]
        [InlineData("Development")]
        [InlineData("Staging")]
        [InlineData("Production")]
        [InlineData("Testing")]
        public void CreateHostBuilder_Should_Work_With_Different_Environments(string environment)
        {
            // Arrange
            string[] args = { "--environment", environment };

            try
            {
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);

                // Act
                var hostBuilder = Program.CreateHostBuilder(args);
                var host = hostBuilder.Build();

                // Assert
                Assert.NotNull(host);
                
                var webHostEnvironment = host.Services.GetRequiredService<IWebHostEnvironment>();
                Assert.Equal(environment, webHostEnvironment.EnvironmentName);
            }
            finally
            {
                // Cleanup
                Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", null);
            }
        }
    }
}

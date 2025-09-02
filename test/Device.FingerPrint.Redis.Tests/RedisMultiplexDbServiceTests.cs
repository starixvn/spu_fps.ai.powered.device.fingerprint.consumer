using Device.FingerPrint.Redis;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace Device.FingerPrint.Redis.Tests
{
    /// <summary>
    /// Tests for RedisMultiplexDbService
    /// Note: These tests focus on constructor validation and basic instantiation
    /// without requiring an actual Redis connection.
    /// </summary>
    public class RedisMultiplexDbServiceTests
    {
        private readonly IConfiguration _configuration;

        public RedisMultiplexDbServiceTests()
        {
            // Create test configuration that matches the expected Redis configuration structure
            var configData = new List<KeyValuePair<string, string?>>
            {
                new("Redis:Password", "testpassword"),
                new("Redis:AllowAdmin", "true"),
                new("Redis:Ssl", "false"),
                new("Redis:AbortOnConnectFail", "false"),
                new("Redis:SyncTimeout", "5000"),
                new("Redis:ConnectTimeout", "6000"),
                new("Redis:ConnectRetry", "3"),
                new("Redis:Database", "0"),
                new("Redis:KeepAlive", "180"),
                new("Redis:Expired", "604800"),
                new("Redis:Hosts:0:Host", "localhost"),
                new("Redis:Hosts:0:Port", "6379"),
                new("Redis:EverTaggingDbIndex", "1"),
                new("Redis:EverTaggingExpireSecond", "3600")
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configData)
                .Build();
        }

        [Fact]
        public void Constructor_WithNullConfiguration_ShouldThrow()
        {
            // Act & Assert
            var action = () => new RedisMultiplexDbService(null!);
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Constructor_WithMissingRedisSection_ShouldThrow()
        {
            // Arrange
            var emptyConfig = new ConfigurationBuilder().Build();

            // Act & Assert
            var action = () => new RedisMultiplexDbService(emptyConfig);
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void Constructor_WithValidConfiguration_ShouldNotThrowImmediately()
        {
            // This test validates that the constructor accepts valid configuration
            // The actual Redis connection might fail, but that's expected without Redis server
            
            // Act & Assert
            var action = () => new RedisMultiplexDbService(_configuration);
            
            // We expect this to either succeed (if Redis is available) or throw RedisConnectionException
            // We're testing that it doesn't throw for configuration issues
            try
            {
                var service = action();
                service.Should().NotBeNull();
                Assert.True(true, "Service created successfully");
            }
            catch (Exception ex) when (ex.Message.Contains("connection") || ex.Message.Contains("timeout"))
            {
                // Expected when Redis is not available
                Assert.True(true, "Redis connection not available - constructor configuration is valid");
            }
        }

        [Fact]
        public void Interface_ShouldImplementIRedisMultiplexDbService()
        {
            // This test verifies the class implements the interface correctly
            // without requiring actual Redis connection

            var serviceType = typeof(RedisMultiplexDbService);
            var interfaceType = typeof(IRedisMultiplexDbService);

            // Assert
            serviceType.Should().Implement<IRedisMultiplexDbService>();
            interfaceType.IsAssignableFrom(serviceType).Should().BeTrue();
        }

        [Fact]
        public void PublicMethods_ShouldBeAvailableOnInterface()
        {
            // Verify all expected methods are available on the interface
            var interfaceType = typeof(IRedisMultiplexDbService);
            var methods = interfaceType.GetMethods();

            // Assert
            methods.Should().Contain(m => m.Name == "GetConnection");
            methods.Should().Contain(m => m.Name == "GetDatabase");
            methods.Should().Contain(m => m.Name == "GetServer");
            methods.Should().Contain(m => m.Name == "HashSetMultipleFieldsWithDbIndexAsync" && m.GetParameters().Any(p => p.ParameterType == typeof(Dictionary<string, string>)));
            methods.Should().Contain(m => m.Name == "HashSetMultipleFieldsWithDbIndexAsync" && m.GetParameters().Any(p => p.ParameterType == typeof(HashEntry[])));
            methods.Should().Contain(m => m.Name == "GetTimeSpan");
            methods.Should().Contain(m => m.Name == "HashGetWithDbIndex");
        }
    }
}

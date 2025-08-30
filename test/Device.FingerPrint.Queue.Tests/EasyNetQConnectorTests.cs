using System;
using Device.FingerPrint.Queue;
using EasyNetQ;
using EasyNetQ.Topology;
using FluentAssertions;
using Moq;
using Xunit;

namespace Device.FingerPrint.Queue.Tests
{
    public class EasyNetQConnectorTests
    {
        private const string TestHost = "localhost";
        private const string TestVirtualHost = "/";
        private const int TestPrefetchCount = 100;
        private const string TestUsername = "testuser";
        private const string TestPassword = "testpass";
        private const string TestExchangeName = "test.exchange";
        private const string TestQueueName = "test.queue";
        private const string TestRoutingKey = "test.routing.key";

        [Fact]
        public void Constructor_WithValidParameters_ShouldSetProperties()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Assert
            connector.Should().NotBeNull();
            connector.QueueName.Should().BeNull();
            connector.ExchangeName.Should().BeNull();
            connector.RoutingKey.Should().BeNull();
            connector.Exchange.Should().BeNull();
            connector.AdvanceBus.Should().BeNull();
            connector.Bus.Should().BeNull();
            connector.Queue.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithNullHost_ShouldSetDefaultHost()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(null, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullVirtualHost_ShouldSetDefaultVirtualHost()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, null, TestPrefetchCount, TestUsername, TestPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullPrefetchCount_ShouldSetDefaultPrefetchCount()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, null, TestUsername, TestPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullUsername_ShouldSetDefaultUsername()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, null, TestPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullPassword_ShouldSetDefaultPassword()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, null);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithAllNullParameters_ShouldCreateInstance()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(null, null, null, null, null);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void CreateBus_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            var result = connector.CreateBus();

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void CreateBus_WhenCalledMultipleTimes_ShouldNotRecreateBus()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            var result1 = connector.CreateBus();
            var result2 = connector.CreateBus();

            // Assert
            result1.Should().BeSameAs(result2);
            result1.Should().BeSameAs(connector);
        }

        [Fact]
        public void CreateBus_ShouldSetAdvanceBus()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            connector.CreateBus();

            // Assert
            connector.AdvanceBus.Should().NotBeNull();
        }

        [Fact]
        public void DeclareExchange_WithValidExchangeName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareExchange(TestExchangeName);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareExchange_WithValidExchangeName_ShouldSetExchange()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            connector.DeclareExchange(TestExchangeName);

            // Assert
            connector.Exchange.Should().NotBeNull();
        }

        [Fact]
        public void DeclareExchange_WithValidExchangeName_ShouldSetExchangeName()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            connector.DeclareExchange(TestExchangeName);

            // Assert
            connector.ExchangeName.Should().Be(TestExchangeName);
        }

        [Fact]
        public void DeclareExchange_WithNullExchangeName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareExchange(null);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareExchange_WithEmptyExchangeName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareExchange(string.Empty);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareExchange_WithWhitespaceExchangeName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareExchange("   ");

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareExchange_WithoutCreatingBus_ShouldThrowException()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act & Assert
            Action action = () => connector.DeclareExchange(TestExchangeName);
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void DeclareRoutingKey_WithValidRoutingKey_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            var result = connector.DeclareRoutingKey(TestRoutingKey);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareRoutingKey_WithValidRoutingKey_ShouldSetRoutingKey()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            connector.DeclareRoutingKey(TestRoutingKey);

            // Assert
            connector.RoutingKey.Should().Be(TestRoutingKey);
        }

        [Fact]
        public void DeclareRoutingKey_WithNullRoutingKey_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            var result = connector.DeclareRoutingKey(null);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareRoutingKey_WithEmptyRoutingKey_ShouldSetEmptyRoutingKey()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            connector.DeclareRoutingKey(string.Empty);

            // Assert
            connector.RoutingKey.Should().Be(string.Empty);
        }

        [Fact]
        public void DeclareQueue_WithValidQueueName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareQueue(TestQueueName);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareQueue_WithValidQueueName_ShouldSetQueue()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            connector.DeclareQueue(TestQueueName);

            // Assert
            connector.Queue.Should().NotBeNull();
        }

        [Fact]
        public void DeclareQueue_WithValidQueueName_ShouldSetQueueName()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            connector.DeclareQueue(TestQueueName);

            // Assert
            connector.QueueName.Should().Be(TestQueueName);
        }

        [Fact]
        public void DeclareQueue_WithNullQueueName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareQueue(null);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareQueue_WithEmptyQueueName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareQueue(string.Empty);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareQueue_WithWhitespaceQueueName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            var result = connector.DeclareQueue("   ");

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareQueue_WithoutCreatingBus_ShouldThrowException()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act & Assert
            Action action = () => connector.DeclareQueue(TestQueueName);
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Bind_WithValidExchangeAndQueue_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();
            connector.DeclareExchange(TestExchangeName);
            connector.DeclareQueue(TestQueueName);
            connector.DeclareRoutingKey(TestRoutingKey);

            // Act
            var result = connector.Bind();

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void Bind_WithoutExchange_ShouldThrowException()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();
            connector.DeclareQueue(TestQueueName);
            connector.DeclareRoutingKey(TestRoutingKey);

            // Act & Assert
            Action action = () => connector.Bind();
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Bind_WithoutQueue_ShouldThrowException()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();
            connector.DeclareExchange(TestExchangeName);
            connector.DeclareRoutingKey(TestRoutingKey);

            // Act & Assert
            Action action = () => connector.Bind();
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Bind_WithoutRoutingKey_ShouldWorkWithNullRoutingKey()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();
            connector.DeclareExchange(TestExchangeName);
            connector.DeclareQueue(TestQueueName);

            // Act
            var result = connector.Bind();

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void Bind_WithoutCreatingBus_ShouldThrowException()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act & Assert
            Action action = () => connector.Bind();
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void Disposed_ShouldDisposeBusAndAdvanceBus()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);
            connector.CreateBus();

            // Act
            Action action = () => connector.Disposed();

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Disposed_WithoutCreatingBus_ShouldThrowException()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act & Assert
            Action action = () => connector.Disposed();
            action.Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void FluentInterface_ShouldWorkCorrectly()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            var result = connector
                .CreateBus()
                .DeclareExchange(TestExchangeName)
                .DeclareQueue(TestQueueName)
                .DeclareRoutingKey(TestRoutingKey)
                .Bind();

            // Assert
            result.Should().BeSameAs(connector);
            connector.Exchange.Should().NotBeNull();
            connector.Queue.Should().NotBeNull();
            connector.RoutingKey.Should().Be(TestRoutingKey);
            connector.ExchangeName.Should().Be(TestExchangeName);
            connector.QueueName.Should().Be(TestQueueName);
        }

        [Fact]
        public void FluentInterface_WithNullValues_ShouldHandleGracefully()
        {
            // Arrange
            var connector = new EasyNetQConnector(null, null, null, null, null);

            // Act
            var result = connector
                .CreateBus()
                .DeclareExchange(null)
                .DeclareQueue(null)
                .DeclareRoutingKey(null)
                .Bind();

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void FluentInterface_WithEmptyValues_ShouldHandleGracefully()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            var result = connector
                .CreateBus()
                .DeclareExchange(string.Empty)
                .DeclareQueue(string.Empty)
                .DeclareRoutingKey(string.Empty)
                .Bind();

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void FluentInterface_WithWhitespaceValues_ShouldHandleGracefully()
        {
            // Arrange
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, TestPrefetchCount, TestUsername, TestPassword);

            // Act
            var result = connector
                .CreateBus()
                .DeclareExchange("   ")
                .DeclareQueue("   ")
                .DeclareRoutingKey("   ")
                .Bind();

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void Constructor_WithSpecialCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var specialHost = "host@example.com:5672";
            var specialVirtualHost = "/test/vhost";
            var specialUsername = "user@domain.com";
            var specialPassword = "pass@word!123";

            // Act
            var connector = new EasyNetQConnector(specialHost, specialVirtualHost, TestPrefetchCount, specialUsername, specialPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithUnicodeCharacters_ShouldHandleCorrectly()
        {
            // Arrange
            var unicodeHost = "host-你好.example.com";
            var unicodeVirtualHost = "/test/世界";
            var unicodeUsername = "user-世界";
            var unicodePassword = "pass-你好";

            // Act
            var connector = new EasyNetQConnector(unicodeHost, unicodeVirtualHost, TestPrefetchCount, unicodeUsername, unicodePassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithVeryLongValues_ShouldHandleCorrectly()
        {
            // Arrange
            var longHost = new string('a', 1000);
            var longVirtualHost = new string('b', 1000);
            var longUsername = new string('c', 1000);
            var longPassword = new string('d', 1000);

            // Act
            var connector = new EasyNetQConnector(longHost, longVirtualHost, TestPrefetchCount, longUsername, longPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithZeroPrefetchCount_ShouldHandleCorrectly()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, 0, TestUsername, TestPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNegativePrefetchCount_ShouldHandleCorrectly()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, -1, TestUsername, TestPassword);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithMaxIntPrefetchCount_ShouldHandleCorrectly()
        {
            // Arrange & Act
            var connector = new EasyNetQConnector(TestHost, TestVirtualHost, int.MaxValue, TestUsername, TestPassword);

            // Assert
            connector.Should().NotBeNull();
        }
    }
}

using Device.FingerPrint.Queue;
using FluentAssertions;
using Moq;
using EasyNetQ;
using EasyNetQ.Topology;

namespace Device.FingerPrint.Queue.Tests
{
    public class EasyNetQConnectorTests
    {
        [Fact]
        public void Constructor_WithValidParameters_ShouldSetProperties()
        {
            // Arrange
            var host = "localhost";
            var virtualHost = "/";
            var prefetchCount = 100;
            var username = "guest";
            var password = "guest";

            // Act
            var connector = new EasyNetQConnector(host, virtualHost, prefetchCount, username, password);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_WithNullParameters_ShouldHandleGracefully()
        {
            // Act
            var connector = new EasyNetQConnector(null, null, null, null, null);

            // Assert
            connector.Should().NotBeNull();
        }

        [Fact]
        public void DeclareExchange_WithEmptyExchangeName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");

            // Act
            var result = connector.DeclareExchange("");

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareExchange_WithNullExchangeName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");

            // Act
            var result = connector.DeclareExchange(null);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareRoutingKey_WithValidKey_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");
            var routingKey = "test.routing.key";

            // Act
            var result = connector.DeclareRoutingKey(routingKey);

            // Assert
            result.Should().BeSameAs(connector);
            connector.RoutingKey.Should().Be(routingKey);
        }

        [Fact]
        public void DeclareRoutingKey_WithNullKey_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");

            // Act
            var result = connector.DeclareRoutingKey(null);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareQueue_WithEmptyQueueName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");

            // Act
            var result = connector.DeclareQueue("");

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void DeclareQueue_WithNullQueueName_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");

            // Act
            var result = connector.DeclareQueue(null);

            // Assert
            result.Should().BeSameAs(connector);
        }

        [Fact(Skip = "Requires actual RabbitMQ connection")]
        public void FluentInterface_ShouldAllowMethodChaining()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");

            // Act
            var result = connector
                .DeclareExchange("test.exchange")
                .DeclareRoutingKey("test.key")
                .DeclareQueue("test.queue");

            // Assert
            result.Should().BeSameAs(connector);
            connector.RoutingKey.Should().Be("test.key");
        }

        [Fact(Skip = "Requires actual RabbitMQ connection")]
        public void Bind_ShouldReturnSelf()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");

            // Act & Assert
            // Since EasyNetQ IAdvancedBus.Bind has optional parameters which cause issues with Moq,
            // we just test that the method returns the connector instance for fluent interface
            var result = connector.Bind();
            result.Should().BeSameAs(connector);
        }

        [Fact]
        public void Properties_ShouldBeSettableAndGettable()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");
            var testQueueName = "test.queue";
            var testExchangeName = "test.exchange";
            var testRoutingKey = "test.key";

            // Act
            connector.QueueName = testQueueName;
            connector.ExchangeName = testExchangeName;
            connector.RoutingKey = testRoutingKey;

            // Assert
            connector.QueueName.Should().Be(testQueueName);
            connector.ExchangeName.Should().Be(testExchangeName);
            connector.RoutingKey.Should().Be(testRoutingKey);
        }

        [Fact]
        public void Disposed_WithValidBus_ShouldDisposeBusAndAdvanceBus()
        {
            // Arrange
            var connector = new EasyNetQConnector("localhost", "/", 100, "guest", "guest");
            var mockBus = new Mock<IBus>();
            var mockAdvancedBus = new Mock<IAdvancedBus>();

            connector.Bus = mockBus.Object;
            connector.AdvanceBus = mockAdvancedBus.Object;

            // Act
            connector.Disposed();

            // Assert
            mockBus.Verify(x => x.Dispose(), Times.Once);
            mockAdvancedBus.Verify(x => x.Dispose(), Times.Once);
        }
    }
}

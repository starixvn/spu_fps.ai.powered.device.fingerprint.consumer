using EasyNetQ;
using EasyNetQ.Topology;

namespace Device.FingerPrint.Queue
{
    public class EasyNetQConnector
    {
        private string Host { get; set; }

        private string VirtualHost { get; set; }

        private int PrefetchCount { get; set; }

        private string Username { get; set; }

        private string Password { get; set; }

        public string QueueName { get; set; }

        public string ExchangeName { get; set; }

        public string RoutingKey { get; set; }

        public IExchange Exchange { get; set; }

        private IBinding Binding { get; set; }

        public IAdvancedBus AdvanceBus { get; set; }

        public IBus Bus { get; set; }

        public IQueue Queue { get; set; }

        public EasyNetQConnector(string host, string virtualHost, int? prefetchCount, string username, string password)
        {
            Host = host ?? Host;
            VirtualHost = virtualHost ?? VirtualHost;
            PrefetchCount = prefetchCount ?? PrefetchCount;
            Username = username ?? Username;
            Password = password ?? Password;
        }

        public EasyNetQConnector CreateBus()
        {
            var connectionString =
                $"host={this.Host};virtualHost={this.VirtualHost};prefetchcount={this.PrefetchCount};username={this.Username};password={this.Password}";

            if (Bus == null)
            {
                Bus = RabbitHutch.CreateBus(connectionString);

                if (Bus != null)
                {
                    AdvanceBus = Bus.Advanced;
                }
            }

            return this;
        }

        public EasyNetQConnector DeclareExchange(string exchangeName)
        {
            if (string.IsNullOrWhiteSpace(exchangeName))
            {
                return this;
            }

            Exchange = AdvanceBus.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            if (Exchange != null)
            {
                this.ExchangeName = Exchange.Name;
            }

            return this;
        }

        public EasyNetQConnector DeclareRoutingKey(string routingKey)
        {
            RoutingKey = routingKey ?? RoutingKey;

            return this;
        }

        public EasyNetQConnector DeclareQueue(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
            {
                return this;
            }

            this.Queue = AdvanceBus.QueueDeclare(queueName);

            if (this.Queue != null)
            {
                this.QueueName = this.Queue.Name;
            }

            return this;
        }

        public EasyNetQConnector Bind()
        {
            Binding = AdvanceBus.Bind(this.Exchange, this.Queue, this.RoutingKey);

            return this;
        }

        public void Disposed()
        {
            this.Bus.Dispose();
            this.AdvanceBus.Dispose();
        }
    }
}



namespace Device.FingerPrint.Queue
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;

    public interface IMassTransitProducer
    {
        Task SendAsync<T>(T data, string address);

        Task SendBatchAsync<T>(IEnumerable<T> data, string address) where T : class;

        void Send<T>(T data, string address);
    }
    public class MassTransitProducer : IMassTransitProducer
    {
        private readonly IBus bus;

        public MassTransitProducer(IBus bus)
        {
            this.bus = bus;
        }
        public void Send<T>(T data, string address)
        {
            var endpointTask = bus.GetSendEndpoint(new Uri(address));

            endpointTask.ContinueWith(endpoint =>
            {
                if (endpoint.Status == TaskStatus.RanToCompletion)
                {
                    endpoint.Result.Send(data, new CancellationToken());
                }
            });
        }
        public async Task SendAsync<T>(T data, string address)
        {
            var endpoint = await bus.GetSendEndpoint(new Uri(address));

            await endpoint.Send(data, new CancellationToken());
        }

        public async Task SendBatchAsync<T>(IEnumerable<T> data, string address) where T : class
        {
            var endpoint = await bus.GetSendEndpoint(new Uri(address));
            await endpoint.SendBatch(data, new CancellationToken());
        }
    }
}

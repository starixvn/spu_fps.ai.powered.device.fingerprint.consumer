using EasyNetQ;
using EasyNetQ.Topology;
using System;
using System.Threading.Tasks;

namespace Device.FingerPrint.Queue
{
    public static class RabbitAdvancedBusConsumeExtension
    {
        public static IDisposable Consume(this IAdvancedBus bus, IQueue queue, Action<byte[], MessageProperties, MessageReceivedInfo> onMessage)
        {
            return bus.Consume(queue, (bytes, properties, info) => ExecuteSynchronously(() => onMessage(bytes, properties, info)));
        }

        public static IDisposable Consume(this IAdvancedBus bus, IQueue queue, Action<byte[], MessageProperties, MessageReceivedInfo> onMessage, Action<IConsumerConfiguration> configure)
        {
            return bus.Consume(queue, (bytes, properties, info) => ExecuteSynchronously(() => onMessage(bytes, properties, info)), configure);
        }

        private static Task ExecuteSynchronously(Action action)
        {
            var tcs = new TaskCompletionSource<object>();

            try
            {
                action();

                tcs.SetResult(null);
            }
            catch (Exception e)
            {
                tcs.SetException(e);
            }

            return tcs.Task;
        }
    }
}

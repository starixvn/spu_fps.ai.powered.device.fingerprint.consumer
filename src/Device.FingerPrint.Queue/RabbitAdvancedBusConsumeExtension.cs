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
            // Use dynamic to call the original IAdvancedBus.Consume method, bypassing extension method resolution
            dynamic dynamicBus = bus;
            Func<byte[], MessageProperties, MessageReceivedInfo, Task> messageHandler = 
                (bytes, properties, info) => ExecuteSynchronously(() => onMessage(bytes, properties, info));
            
            return dynamicBus.Consume(queue, messageHandler);
        }

        public static IDisposable Consume(this IAdvancedBus bus, IQueue queue, Action<byte[], MessageProperties, MessageReceivedInfo> onMessage, Action<IConsumerConfiguration> configure)
        {
            // Use dynamic to call the original IAdvancedBus.Consume method, bypassing extension method resolution
            dynamic dynamicBus = bus;
            Func<byte[], MessageProperties, MessageReceivedInfo, Task> messageHandler = 
                (bytes, properties, info) => ExecuteSynchronously(() => onMessage(bytes, properties, info));
            
            return dynamicBus.Consume(queue, messageHandler, configure);
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

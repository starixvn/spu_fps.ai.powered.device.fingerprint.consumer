using Device.FingerPrint.Features.Monitor.Services;
using Device.FingerPrint.Features.Transaction.Services;
using Device.FingerPrint.Queue;
using Device.FingerPrint.QueueConsumer.Handlers;
using Microsoft.Extensions.DependencyInjection;

namespace Device.FingerPrint.QueueConsumer.Services
{
    public static class InitServices
    {
        public static void RegisterService(this IServiceCollection services)
        {
            RegisterTransactionService(services);
            RegisterDeviceService(services);
            RegisterMassTransitService(services);
            RegisterBotDetectionService(services);
        }

        private static void RegisterTransactionService(this IServiceCollection services)
        {
            services.AddSingleton<ITransactionDataAccess, TransactionDataAccess>();
            services.AddSingleton<ITransactionService, TransactionService>();

            services.AddSingleton<ITransactionReportDataAccess, TransactionReportDataAccess>();
            services.AddSingleton<ITransactionReportServices, TransactionReportServices>();
        }

        private static void RegisterDeviceService(this IServiceCollection services)
        {
            services.AddSingleton<IDeviceService, DeviceService>();
        }

        private static void RegisterMassTransitService(this IServiceCollection services)
        {
            services.AddSingleton<IMassTransitProducer, MassTransitProducer>();
            services.AddSingleton<Consumer>();
        }

        private static void RegisterBotDetectionService(this IServiceCollection services)
        {
            services.AddSingleton<IBotDetectionService, BotDetectionService>();
        }
    }
}

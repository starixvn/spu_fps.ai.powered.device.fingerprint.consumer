namespace Device.FingerPrint.QueueConsumer
{
    using Device.FingerPrint.Core;
    using Device.FingerPrint.Core.Services;
    using Device.FingerPrint.QueueConsumer.Handlers;
    using Device.FingerPrint.QueueConsumer.Services;
    using Device.FingerPrint.Redis;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.AspNetCore;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.Sentry;
    using MassTransit;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics.HealthChecks;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Sentry;
    using System;
    using System.Collections.Generic;

    public class Startup(IConfiguration configuration)
    {
        public IConfiguration Configuration { get; } = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var appSettings = new AppSettings(this.Configuration);

            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<ILogger, Logger>();

            services.AddHealthChecks();

            services.RegisterService();

            services.AddFanexSql(this.Configuration);

            LogManager
               .SetDefaultLogCategory("Device.FingerPrint.Consumer")
               .Use(new List<ILogEngine>
               {
                        new SentryLogging(new SentryEngineOptions
                        {
                            Dsn = new Dsn(appSettings.SentryDsn)
                        })
               });

            ExceptionHandler
                .UseConfig(new LoggingConfiguration())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            RegisterRedis(services);

            InitService(services);

            services.AddMassTransit(busConfigurator =>
            {
                busConfigurator.SetKebabCaseEndpointNameFormatter();

                busConfigurator.AddConsumer<Consumer>(cfg =>
                {
                    cfg.Options<BatchOptions>(options => options
                    .SetMessageLimit(appSettings.GetValue<int>("Consumer:Masstransit:BatchSizeLimit"))
                    .SetTimeLimit(s: appSettings.GetValue<int>("Consumer:Masstransit:BatchTimeLimit"))
                    .SetConcurrencyLimit(appSettings.GetValue<int>("Consumer:Masstransit:BatchConcurrencyLimit")));

                    cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                }).Endpoint(e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Name = appSettings.GetValue<string>("Consumer:Name");
                    e.PrefetchCount = appSettings.GetValue<int>("Consumer:Masstransit:PrefetchCount");
                });


                busConfigurator.UsingRabbitMq((context, rgx) =>
                {
                    rgx.ConfigureEndpoints(context);
                });
            });

            services.AddOptions<RabbitMqTransportOptions>()
            .Configure(options =>
            {
                options.Host = appSettings.GetValue<string>("RabbitMQ:Host");
                options.Port = appSettings.GetValue<ushort>("RabbitMQ:Port");
                options.User = appSettings.GetValue<string>("RabbitMQ:UserName");
                options.Pass = appSettings.GetValue<string>("RabbitMQ:Password");
                options.VHost = appSettings.GetValue<string>("RabbitMQ:VirtualHost");
            });

            LogService.Info($"QueueConsumer {appSettings.Environment} Start with ValueEnable({appSettings.IsVaultEnable})");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHealthChecks("/healthz", new HealthCheckOptions
            {
                Predicate = _ => true
            });

            app.UseRouting();
        }

        private static void RegisterRedis(IServiceCollection services)
        {
            services.AddSingleton<IRedisMultiplexDbService, RedisMultiplexDbService>();
        }

        private void InitService(IServiceCollection services)
        {
            services.AddSingleton<IRawTransactionService, RawTransactionService>();
            services.AddSingleton<ICrypto, Crypto>();
            services.AddSingleton<IUserAgentService, UserAgentService>();
            services.AddSingleton<IMobileRawTransactionService, MobileRawTransactionService>();
        }
    }
}

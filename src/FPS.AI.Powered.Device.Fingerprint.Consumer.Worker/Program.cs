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
using Microsoft.Extensions.Configuration;
using Sentry;

var builder = Host.CreateApplicationBuilder(args);

// Configure app configuration
builder.Configuration.AddEnvironmentVariables();
builder.Configuration.SetBasePath(builder.Environment.ContentRootPath);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// Configure services
var appSettings = new AppSettings(builder.Configuration);

builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddSingleton<Fanex.Logging.ILogger, Logger>();

builder.Services.AddHealthChecks();

builder.Services.RegisterService();

Console.WriteLine($"StoredProcedurePath: {builder.Configuration.GetSection("StoredProcedurePath").Value}");
// Debug App_Data file
Console.WriteLine("App_Data files:");
var appDataPath = Path.Combine(AppContext.BaseDirectory, "App_Data", "UAT");
if (Directory.Exists(appDataPath))
{
    var files = Directory.GetFiles(appDataPath);
    foreach (var file in files)
    {
        Console.WriteLine($" - {Path.GetFileName(file)}");
    }
}
else
{
    Console.WriteLine("App_Data directory does not exist.");
}

builder.Services.AddFanexSql(builder.Configuration);

// Configure logging
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

// Register Redis
builder.Services.AddSingleton<IRedisMultiplexDbService, RedisMultiplexDbService>();

// Initialize services
builder.Services.AddSingleton<IRawTransactionService, RawTransactionService>();
builder.Services.AddSingleton<IRawTransactionAttributeService, RawTransactionAttributeService>();
builder.Services.AddSingleton<ICrypto, Crypto>();
builder.Services.AddSingleton<IUserAgentService, UserAgentService>();
builder.Services.AddSingleton<IMobileRawTransactionService, MobileRawTransactionService>();

// Configure MassTransit
builder.Services.AddMassTransit(busConfigurator =>
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

builder.Services.AddOptions<RabbitMqTransportOptions>()
.Configure(options =>
{
    options.Host = appSettings.GetValue<string>("RabbitMQ:Host");
    options.Port = appSettings.GetValue<ushort>("RabbitMQ:Port");
    options.User = appSettings.GetValue<string>("RabbitMQ:UserName");
    options.Pass = appSettings.GetValue<string>("RabbitMQ:Password");
    options.VHost = appSettings.GetValue<string>("RabbitMQ:VirtualHost");
});

// Add hosted service - commented out as not needed

LogService.Info($"QueueConsumer {appSettings.Environment} Start with ValueEnable({appSettings.IsVaultEnable})");

var host = builder.Build();
host.Run();

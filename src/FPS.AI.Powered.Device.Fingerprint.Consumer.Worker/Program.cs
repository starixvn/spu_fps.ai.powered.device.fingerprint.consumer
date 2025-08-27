using FPS.AI.Powered.Device.Fingerprint.Worker;
using static MassTransit.MessageHeaders;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
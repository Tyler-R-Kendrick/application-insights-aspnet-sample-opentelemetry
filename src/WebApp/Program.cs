using WebApp;
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplicationInsightsTelemetryWorkerService();
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<ExceptionWorker>();
builder.Services.AddOpenTelemetry().UseAzureMonitor();
var host = builder.Build();
host.Run();

using System.Diagnostics.Tracing;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<LoggingOpenTelemetryListener>();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("dotnet-coded-instrumentation"))
    .WithTracing(tracingBuilder => tracingBuilder
        .AddOtlpExporter()
        .AddAspNetCoreInstrumentation())
    .WithMetrics(metricsBuilder => metricsBuilder
        .AddOtlpExporter()
        .AddAspNetCoreInstrumentation());

var app = builder.Build();

var openTelemetryDebugLogger = app.Services.GetRequiredService<LoggingOpenTelemetryListener>();

app.MapGet("/", () => "Hello World!");
app.MapGet("/live", () => "ok");

app.Run();

public class LoggingOpenTelemetryListener : EventListener
{
    private readonly ILogger<LoggingOpenTelemetryListener> _logger;
 
    public LoggingOpenTelemetryListener(ILogger<LoggingOpenTelemetryListener> logger)
    {
        _logger = logger;
    }
    protected override void OnEventSourceCreated(EventSource eventSource)
    {
        if (eventSource.Name.StartsWith("OpenTelemetry"))
            EnableEvents(eventSource, EventLevel.Error);
    }
 
    protected override void OnEventWritten(EventWrittenEventArgs eventData)
    {
        _logger.LogWarning(eventData.Message, eventData.Payload?.Select(p => p?.ToString())?.ToArray()!);
    }
}

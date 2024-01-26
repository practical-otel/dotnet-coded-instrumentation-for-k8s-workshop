using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("dotnet-coded-instrumentation"))
    .WithTracing(tracingBuilder => tracingBuilder
        .AddOtlpExporter()
        .AddAspNetCoreInstrumentation())
    .WithMetrics(metricsBuilder => metricsBuilder
        .AddOtlpExporter()
        .AddAspNetCoreInstrumentation());

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGet("/live", () => "ok");

app.Run();

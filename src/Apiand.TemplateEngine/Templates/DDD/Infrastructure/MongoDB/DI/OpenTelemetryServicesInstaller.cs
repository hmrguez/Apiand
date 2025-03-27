using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace XXXnameXXX.Infrastructure.DI;

public static class OpenTelemetryServicesInstaller
{
    public static void AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var exporterUri = configuration["Logging:OpenTelemetryExporterUri"] ?? "http://localhost:19082";
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("Storm.Api"))
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                metrics.AddOtlpExporter(builder => builder.Endpoint = new Uri(exporterUri));
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                tracing.AddOtlpExporter(builder => builder.Endpoint = new Uri(exporterUri));
            });
    }

    public static void AddOpenTelemetry(this ILoggingBuilder loggingBuilder, IConfiguration configuration)
    {
        var exporterUri = configuration["Logging:OpenTelemetryExporterUri"] ?? "http://localhost:19082";
        loggingBuilder.AddOpenTelemetry(logging =>
        {
            logging.IncludeFormattedMessage = true;
            logging.IncludeScopes = true;
            logging.AddOtlpExporter(builder => builder.Endpoint = new Uri(exporterUri));
        });
    }
}
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using StockMarketLive.Application.Settings;

namespace StockMarketLive.Api.Extensions;

public static class TelemetryExtensions
{
    public static IServiceCollection AddApiTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        var telemetrySettings = new TelemetrySettings();
        configuration.GetSection(TelemetrySettings.SectionName).Bind(telemetrySettings);

        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(serviceName: telemetrySettings.ServiceName, serviceVersion: "1.0.0");

        services.AddOpenTelemetry()
            .WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(resourceBuilder)
                       .AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddEntityFrameworkCoreInstrumentation()
                       .AddSource("MassTransit"); // For RabbitMQ distributed tracing

                if (!string.IsNullOrEmpty(telemetrySettings.OtlpEndpoint))
                {
                    tracing.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(telemetrySettings.OtlpEndpoint);
                        if (!string.IsNullOrEmpty(telemetrySettings.OtlpHeaders))
                        {
                            options.Headers = telemetrySettings.OtlpHeaders;
                        }
                    });
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(resourceBuilder)
                       .AddAspNetCoreInstrumentation()
                       .AddHttpClientInstrumentation()
                       .AddRuntimeInstrumentation();

                if (!string.IsNullOrEmpty(telemetrySettings.OtlpEndpoint))
                {
                    metrics.AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(telemetrySettings.OtlpEndpoint);
                        if (!string.IsNullOrEmpty(telemetrySettings.OtlpHeaders))
                        {
                            options.Headers = telemetrySettings.OtlpHeaders;
                        }
                    });
                }
            });

        return services;
    }
}

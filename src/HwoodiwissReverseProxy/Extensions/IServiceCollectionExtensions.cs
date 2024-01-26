using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HwoodiwissReverseProxy.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, string componentName, Action<MeterProviderBuilder> configureMetrics, Action<TracerProviderBuilder> configureTraces)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(builder => TelemetryResourceBuilder(builder, componentName))
            .WithMetrics(metrics =>
            {
                configureMetrics(metrics);
                metrics.AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                configureTraces(tracing);
                tracing.AddOtlpExporter();
            });

        static void TelemetryResourceBuilder(ResourceBuilder resourceBuilder, string componentName)
        {
            resourceBuilder
                .AddService(ApplicationMetadata.Name)
                .AddAttributes([
                    new ("service.commit", ApplicationMetadata.GitCommit),
                    new ("service.branch", ApplicationMetadata.GitBranch),
                    new ("service.version", ApplicationMetadata.Version),
                    new ("service.host", Environment.MachineName),
                    new ("service.component", componentName),
                ]);
        }
        
        return services;
    }
}

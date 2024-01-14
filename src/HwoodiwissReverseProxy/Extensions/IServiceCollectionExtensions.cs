using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HwoodiwissReverseProxy.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, string componentName)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(builder => TelemetryResourceBuilder(builder, componentName))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("Yarp.ReverseProxy")
                    .AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddSource("Yarp.ReverseProxy")
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
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

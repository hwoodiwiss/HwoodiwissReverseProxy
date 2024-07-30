using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HwoodiwissReverseProxy.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(TelemetryResourceBuilder)
            .WithMetrics(metrics =>
            {
                metrics.AddRuntimeInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddMeter("Microsoft.AspNetCore.Hosting")
                    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                    .AddMeter("Yarp.ReverseProxy")
                    .AddOtlpExporter();
            })
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSource("Yarp.ReverseProxy")
                    .AddOtlpExporter();
            });

        static void TelemetryResourceBuilder(ResourceBuilder resourceBuilder)
        {
            resourceBuilder
                .AddService(ApplicationMetadata.Name)
                .AddAttributes([
                    new("service.commit", ApplicationMetadata.GitCommit),
                    new("service.branch", ApplicationMetadata.GitBranch),
                    new("service.version", ApplicationMetadata.Version),
                    new("service.host", Environment.MachineName),
                ])
                .AddContainerDetector()
                .AddHostDetector();

        }

        return services;
    }
}

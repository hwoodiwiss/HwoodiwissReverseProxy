using System.Runtime.CompilerServices;
using HwoodiwissReverseProxy.Infrastructure;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy.Extensions;

public static class WebApplicationBuilderExtensions
{
    public const string ProxyComponentName = "ReverseProxy";
    public const string ManagementComponentName = "Management";

    public static WebApplicationBuilder WithName(this WebApplicationBuilder builder, string applicationName)
    {
        builder.Environment.ApplicationName = $"{builder.Environment.ApplicationName}.{applicationName}";

        return builder;
    }

    public static WebApplication ConfigureAndBuild(this WebApplicationBuilder builder)
    {
        builder.WithName("Management");
        builder.Configuration.ConfigureConfiguration();
        var managementUrls = builder.Configuration.GetValue<string>("ManagementUrls");
        managementUrls = string.IsNullOrEmpty(managementUrls) ? "http://*:18265" : managementUrls;

        builder.WebHost.UseUrls(managementUrls);
        builder.ConfigureLogging(builder.Configuration);
        builder.Services.AddOptions();
        builder.Services.ConfigureServices(builder.Configuration);

        return builder.Build();
    }

    public static WebApplication ConfigureProxyAndBuild(this WebApplicationBuilder builder, IProxyConfigProvider proxyConfigProvider)
    {
        builder.WithName("Proxy");
        builder.Configuration.ConfigureConfiguration();
        builder.Services.ConfigureProxyServices(proxyConfigProvider);

        return builder.Build();
    }

    private static WebApplicationBuilder ConfigureLogging(this WebApplicationBuilder builder, ConfigurationManager configuration)
    {
        var loggingBuilder = builder.Logging.AddConfiguration(configuration)
            .AddOpenTelemetry(opt =>
            {
                opt.IncludeScopes = true;
                opt.AddOtlpExporter();
            });

#if DEBUG
        loggingBuilder.AddConsole()
            .AddDebug();

        builder.Services.Configure<ConsoleFormatterOptions>(options =>
        {
            options.IncludeScopes = true;
        });
#endif

        return builder;
    }

    private static IConfigurationBuilder ConfigureConfiguration(this IConfigurationBuilder configurationBuilder)
        => configurationBuilder
            .AddJsonFile("appsettings.Secrets.json", true, true);

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfigurationRoot configurationRoot)
    {
        services.AddOptions();
        services.ConfigureJsonOptions(options =>
        {
            options.SerializerOptions.TypeInfoResolver = ApplicationJsonContext.Default;
        });

        if (RuntimeFeature.IsDynamicCodeSupported)
        {
            services.AddEndpointsApiExplorer();
            services.AddOpenApiDocument(cfg =>
            {
                cfg.DocumentName = "v1";
            });
        }

        // Enables easy named loggers in static contexts
        services.AddKeyedTransient<ILogger>(KeyedService.AnyKey, (sp, key) =>
        {
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return loggerFactory.CreateLogger(key as string ?? (key.ToString() ?? "Unknown"));
        });

        services.AddTelemetry();

        services.AddSingleton<IProxyConfigProvider>(sp => new ConfigurationConfigProvider(sp.GetRequiredService<ILogger<ConfigurationConfigProvider>>(), configurationRoot.GetSection("ReverseProxy")));

        return services;
    }

    public static IServiceCollection ConfigureProxyServices(this IServiceCollection services, IProxyConfigProvider proxyConfigProvider)
    {
        services.AddSingleton(proxyConfigProvider);
        services.AddReverseProxy();

        return services;
    }

    private static IServiceCollection ConfigureJsonOptions(this IServiceCollection services, Action<JsonOptions> configureOptions)
    {
        services.ConfigureHttpJsonOptions(configureOptions);

        services.Configure<JsonOptions>(Constants.PrettyPrintJsonOptionsKey, options =>
        {
            configureOptions(options);
            options.SerializerOptions.WriteIndented = true;
        });

        services.AddKeyedTransient<JsonOptions>(KeyedService.AnyKey, (sp, key) =>
        {
            var optionsSnapshot = sp.GetRequiredService<IOptionsSnapshot<JsonOptions>>();
            var jsonOptions = optionsSnapshot.Get(key.ToString());
            return jsonOptions;
        });

        return services;
    }
}

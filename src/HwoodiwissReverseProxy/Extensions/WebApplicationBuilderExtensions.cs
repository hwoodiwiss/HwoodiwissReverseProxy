using System.IO.Compression;
using System.Reflection;
using Hwoodiwiss.Extensions.Hosting;
using HwoodiwissReverseProxy.Infrastructure;
using Microsoft.AspNetCore.StaticFiles;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy.Extensions;

public static class WebApplicationBuilderExtensions
{
    public const string ProxyComponentName = "ReverseProxy";
    public const string ManagementComponentName = "Management";

    public static HwoodiwissApplicationBuilder ConfigureManagement(this HwoodiwissApplicationBuilder builder, out string managementUrls)
    {
        builder.WithName(ManagementComponentName);
        var managementUrlConfiguration = builder.Configuration.GetValue<string>("ManagementUrls");
        managementUrls = string.IsNullOrEmpty(managementUrlConfiguration) ? "http://*:18265" : managementUrlConfiguration;

        builder.Services.Configure<StaticFileOptions>(options => ConfigureStaticFileContentTypeMappings(options));
        builder.Services.AddSingleton<IProxyConfigProvider>(sp => new ConfigurationConfigProvider(sp.GetRequiredService<ILogger<ConfigurationConfigProvider>>(), builder.Configuration.GetSection("ReverseProxy")));

        return builder;
    }

    public static HwoodiwissApplicationBuilder ConfigureProxy(this HwoodiwissApplicationBuilder builder, IProxyConfigProvider proxyConfigProvider)
    {
        builder.WithName(ProxyComponentName);
        builder.Services.AddSingleton(proxyConfigProvider);
        builder.Services.AddReverseProxy();

        return builder;
    }

    private static HwoodiwissApplicationBuilder WithName(this HwoodiwissApplicationBuilder builder, string applicationName)
    {
        builder.Environment.ApplicationName = $"{builder.Environment.ApplicationName}.{applicationName}";

        return builder;
    }

    private static void ConfigureStaticFileContentTypeMappings(StaticFileOptions options)
    {
        var contentTypeProvider = new FileExtensionContentTypeProvider();
        var buildContentTypeMappings = Assembly.GetExecutingAssembly().GetCustomAttributes<ContentTypeMappingAttribute>();
        var mappings = buildContentTypeMappings.ToDictionary(attr => attr.Pattern, attr => attr.ContentType.TrimStart('*'));
        foreach (var mapping in mappings)
        {
            contentTypeProvider.Mappings.Add(mapping.Key, mapping.Value);
        }

        options.ContentTypeProvider = contentTypeProvider;
    }
}

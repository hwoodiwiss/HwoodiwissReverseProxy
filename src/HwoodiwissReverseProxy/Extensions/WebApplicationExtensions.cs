using System.Runtime.CompilerServices;
using HwoodiwissReverseProxy.Endpoints;
using Microsoft.AspNetCore.StaticFiles;

namespace HwoodiwissReverseProxy.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureRequestPipeline(this WebApplication app, IConfiguration configuration)
    {
        app.UseDefaultFiles();


        if (RuntimeFeature.IsDynamicCodeSupported)
        {
            app.UseOpenApi(cfg =>
            {
                cfg.Path = "/swagger/openapi.json";
            });
        }

        app.MapEndpoints(app.Environment);

        app.UseStaticFiles(configuration);

        return app;
    }

    public static WebApplication ConfigureReverseProxy(this WebApplication app)
    {
        app.MapReverseProxy();

        return app;
    }

    private static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
        => builder
            .MapConfigurationEndpoints(environment)
            .MapHealthEndpoints()
            .MapProxyConfigurationEndpoints();

    private static WebApplication UseStaticFiles(this WebApplication app, IConfiguration configuration)
    {
        var contentTypeProvider = new FileExtensionContentTypeProvider();
        var mappings = configuration.GetSection("ContentTypeMappings").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();
        foreach (var mapping in mappings)
        {
            contentTypeProvider.Mappings.Add(mapping.Key, mapping.Value);
        }

        var opts = new StaticFileOptions { ContentTypeProvider = contentTypeProvider };
        app.UseStaticFiles(opts);

        return app;
    }

}

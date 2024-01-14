using HwoodiwissReverseProxy.Endpoints;

namespace HwoodiwissReverseProxy.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureRequestPipeline(this WebApplication app)
    {
        if (!ApplicationMetadata.IsNativeAot)
        {
            app.UseOpenApi();
            app.UseSwaggerUi();
        }
        
        app.MapEndpoints(app.Environment);
        
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
}

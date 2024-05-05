using HwoodiwissReverseProxy.Extensions;
using Microsoft.AspNetCore.Mvc;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy.Endpoints;

public static class ProxyConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapProxyConfigurationEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/proxy")
            .WithPrettyPrint();

        group.MapGet("/", ([FromServices] IProxyConfigProvider configProvider) => configProvider.GetConfig());

        group.MapGet("/routes", ([FromServices] IProxyConfigProvider configProvider) => configProvider.GetConfig().Routes);

        group.MapGet("/clusters", ([FromServices] IProxyConfigProvider configProvider) => configProvider.GetConfig().Clusters);

        return builder;
    }
}

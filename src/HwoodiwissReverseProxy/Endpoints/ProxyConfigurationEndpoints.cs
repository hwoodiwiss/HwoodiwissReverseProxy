using Hwoodiwiss.Extensions.Hosting.Extensions;
using HwoodiwissReverseProxy.Infrastructure;
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

        group.MapPut("/routes/{routeId}", (string routeId, [FromBody] RouteConfig route, [FromServices] InMemoryProxyConfigProvider provider) =>
        {
            provider.UpdateRoute(route with { RouteId = routeId });
            return Results.NoContent();
        });

        group.MapDelete("/routes/{routeId}", (string routeId, [FromServices] InMemoryProxyConfigProvider provider) =>
        {
            if (!provider.RouteExists(routeId))
            {
                return Results.NotFound();
            }

            provider.DeleteRoute(routeId);
            return Results.NoContent();
        });

        group.MapPut("/clusters/{clusterId}", (string clusterId, [FromBody] ClusterConfig cluster, [FromServices] InMemoryProxyConfigProvider provider) =>
        {
            provider.UpdateCluster(cluster with { ClusterId = clusterId });
            return Results.NoContent();
        });

        group.MapDelete("/clusters/{clusterId}", (string clusterId, [FromServices] InMemoryProxyConfigProvider provider) =>
        {
            if (!provider.ClusterExists(clusterId))
            {
                return Results.NotFound();
            }

            provider.DeleteCluster(clusterId);
            return Results.NoContent();
        });

        return builder;
    }
}


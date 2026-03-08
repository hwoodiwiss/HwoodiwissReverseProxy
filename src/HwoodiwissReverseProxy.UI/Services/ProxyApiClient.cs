using System.Net.Http.Json;
using HwoodiwissReverseProxy.UI.Services.Models;

namespace HwoodiwissReverseProxy.UI.Services;

public sealed class ProxyApiClient(HttpClient http)
{
    public Task<List<RouteModel>?> GetRoutesAsync() =>
        http.GetFromJsonAsync<List<RouteModel>>("/proxy/routes", ProxyApiJsonContext.Default.ListRouteModel);

    public Task<List<ClusterModel>?> GetClustersAsync() =>
        http.GetFromJsonAsync<List<ClusterModel>>("/proxy/clusters", ProxyApiJsonContext.Default.ListClusterModel);

    public Task<HttpResponseMessage> UpsertRouteAsync(string routeId, RouteModel route) =>
        http.PutAsJsonAsync($"/proxy/routes/{Uri.EscapeDataString(routeId)}", route, ProxyApiJsonContext.Default.RouteModel);

    public Task<HttpResponseMessage> DeleteRouteAsync(string routeId) =>
        http.DeleteAsync($"/proxy/routes/{Uri.EscapeDataString(routeId)}");

    public Task<HttpResponseMessage> UpsertClusterAsync(string clusterId, ClusterModel cluster) =>
        http.PutAsJsonAsync($"/proxy/clusters/{Uri.EscapeDataString(clusterId)}", cluster, ProxyApiJsonContext.Default.ClusterModel);

    public Task<HttpResponseMessage> DeleteClusterAsync(string clusterId) =>
        http.DeleteAsync($"/proxy/clusters/{Uri.EscapeDataString(clusterId)}");
}

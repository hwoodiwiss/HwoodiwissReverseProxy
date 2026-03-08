using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy.Infrastructure;

public sealed class InMemoryProxyConfigProvider : IProxyConfigProvider, IDisposable
{
    private readonly object _lock = new();
    private InMemoryProxyConfig _config;

    public InMemoryProxyConfigProvider(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        _config = new InMemoryProxyConfig(routes, clusters);
    }

    public IProxyConfig GetConfig() => _config;

    public void UpdateRoute(RouteConfig route)
    {
        lock (_lock)
        {
            var old = _config;
            var routes = old.Routes
                .Where(r => r.RouteId != route.RouteId)
                .Append(route)
                .ToList();
            _config = new InMemoryProxyConfig(routes, old.Clusters);
            old.SignalChange();
            old.Dispose();
        }
    }

    public void DeleteRoute(string routeId)
    {
        lock (_lock)
        {
            var old = _config;
            var routes = old.Routes.Where(r => r.RouteId != routeId).ToList();
            _config = new InMemoryProxyConfig(routes, old.Clusters);
            old.SignalChange();
            old.Dispose();
        }
    }

    public void UpdateCluster(ClusterConfig cluster)
    {
        lock (_lock)
        {
            var old = _config;
            var clusters = old.Clusters
                .Where(c => c.ClusterId != cluster.ClusterId)
                .Append(cluster)
                .ToList();
            _config = new InMemoryProxyConfig(old.Routes, clusters);
            old.SignalChange();
            old.Dispose();
        }
    }

    public void DeleteCluster(string clusterId)
    {
        lock (_lock)
        {
            var old = _config;
            var clusters = old.Clusters.Where(c => c.ClusterId != clusterId).ToList();
            _config = new InMemoryProxyConfig(old.Routes, clusters);
            old.SignalChange();
            old.Dispose();
        }
    }

    public bool RouteExists(string routeId)
    {
        lock (_lock)
        {
            return _config.Routes.Any(r => r.RouteId == routeId);
        }
    }

    public bool ClusterExists(string clusterId)
    {
        lock (_lock)
        {
            return _config.Clusters.Any(c => c.ClusterId == clusterId);
        }
    }

    public void Dispose()
    {
        _config.Dispose();
    }
}

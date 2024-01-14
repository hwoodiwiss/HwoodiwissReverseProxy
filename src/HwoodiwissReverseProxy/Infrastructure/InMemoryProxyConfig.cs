using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy.Infrastructure;

public sealed class InMemoryProxyConfig : IProxyConfig, IDisposable
{
    // Used to implement the change token for the state
    private readonly CancellationTokenSource _cts = new();

    public InMemoryProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        : this(routes, clusters, Guid.NewGuid().ToString())
    { }

    public InMemoryProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters, string revisionId)
    {
        RevisionId = revisionId ?? throw new ArgumentNullException(nameof(revisionId));
        Routes = routes;
        Clusters = clusters;
        ChangeToken = new CancellationChangeToken(_cts.Token);
    }

    /// <inheritdoc/>
    public string RevisionId { get; }

    /// <summary>
    /// A snapshot of the list of routes for the proxy
    /// </summary>
    public IReadOnlyList<RouteConfig> Routes { get; }

    /// <summary>
    /// A snapshot of the list of Clusters which are collections of interchangable destination endpoints
    /// </summary>
    public IReadOnlyList<ClusterConfig> Clusters { get; }

    /// <summary>
    /// Fired to indicate the the proxy state has changed, and that this snapshot is now stale
    /// </summary>
    public IChangeToken ChangeToken { get; }

    internal void SignalChange()
    {
        _cts.Cancel();
    }

    public void Dispose()
    {
        _cts.Dispose();
    }
}

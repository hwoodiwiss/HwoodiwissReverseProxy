using System.Text.Json.Serialization;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(IProxyConfig))]
[JsonSerializable(typeof(IReadOnlyList<RouteConfig>))]
[JsonSerializable(typeof(IReadOnlyList<ClusterConfig>))]
public partial class ApplicationJsonContext : JsonSerializerContext
{
}


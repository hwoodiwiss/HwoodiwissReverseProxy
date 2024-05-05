using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using HwoodiwissReverseProxy.Infrastructure;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy;

[JsonSerializable(typeof(object))]
[JsonSerializable(typeof(KeyValuePair<string, string>))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(IProxyConfig))]
[JsonSerializable(typeof(JsonObject))]
[JsonSerializable(typeof(RouteConfig))]
[JsonSerializable(typeof(ClusterConfig))]
[JsonSerializable(typeof(ConfigurationSnapshot))]
[JsonSerializable(typeof(List<RouteConfig>))]
[JsonSerializable(typeof(List<ClusterConfig>))]
public partial class ApplicationJsonContext : JsonSerializerContext;

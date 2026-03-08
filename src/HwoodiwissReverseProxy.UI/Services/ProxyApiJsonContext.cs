using System.Net.Http.Json;
using System.Text.Json.Serialization;
using HwoodiwissReverseProxy.UI.Services.Models;

namespace HwoodiwissReverseProxy.UI.Services;

[JsonSerializable(typeof(RouteModel))]
[JsonSerializable(typeof(List<RouteModel>))]
[JsonSerializable(typeof(ClusterModel))]
[JsonSerializable(typeof(List<ClusterModel>))]
[JsonSerializable(typeof(RouteMatchModel))]
[JsonSerializable(typeof(DestinationModel))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public sealed partial class ProxyApiJsonContext : JsonSerializerContext;

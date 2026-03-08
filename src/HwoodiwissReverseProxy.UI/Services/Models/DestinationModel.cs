using System.Text.Json.Serialization;

namespace HwoodiwissReverseProxy.UI.Services.Models;

public sealed class DestinationModel
{
    [JsonRequired]
    public string Address { get; set; } = string.Empty;
    public string? Host { get; set; }
    public string? Health { get; set; }
}

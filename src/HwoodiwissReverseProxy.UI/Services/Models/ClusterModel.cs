using System.ComponentModel.DataAnnotations;

namespace HwoodiwissReverseProxy.UI.Services.Models;

public sealed class ClusterModel
{
    [Required]
    public string ClusterId { get; set; } = string.Empty;
    public string? LoadBalancingPolicy { get; set; }
    public Dictionary<string, DestinationModel> Destinations { get; set; } = new();
}

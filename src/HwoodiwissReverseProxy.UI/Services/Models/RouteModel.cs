using System.ComponentModel.DataAnnotations;

namespace HwoodiwissReverseProxy.UI.Services.Models;

public sealed class RouteModel
{
    [Required]
    public string RouteId { get; set; } = string.Empty;
    public string? ClusterId { get; set; }
    public int? Order { get; set; }
    public RouteMatchModel Match { get; set; } = new();
}

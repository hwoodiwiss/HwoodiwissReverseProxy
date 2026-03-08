namespace HwoodiwissReverseProxy.UI.Services.Models;

public sealed class RouteMatchModel
{
    public string? Path { get; set; }
    public List<string>? Hosts { get; set; }
    public List<string>? Methods { get; set; }
}

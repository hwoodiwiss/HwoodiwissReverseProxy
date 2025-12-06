using Hwoodiwiss.Extensions.Hosting;
using HwoodiwissReverseProxy;
using HwoodiwissReverseProxy.Endpoints;
using HwoodiwissReverseProxy.Extensions;
using Yarp.ReverseProxy.Configuration;

// Management app has to come first so that that is the host that WebApplicationFactory overrides
var mgmt = HwoodiwissApplication
    .CreateBuilder(args)
    .WithHttpJsonContexts(ApplicationJsonContext.Default)
    .ConfigureOptions(opt => opt.HostStaticAssets = true)
    .ConfigureMetrics(meterBuilder => meterBuilder.AddMeter("Yarp.ReverseProxy"))
    .ConfigureTracing(tracerBuilder => tracerBuilder.AddSource("Yarp.ReverseProxy"))
    .ConfigureManagement(out var managementUrls)
    .Build();

mgmt.MapProxyConfigurationEndpoints();

var proxy = HwoodiwissApplication
    .CreateBuilder(args)
    .ConfigureProxy(mgmt.Services.GetRequiredService<IProxyConfigProvider>())
    .Build();

proxy.MapReverseProxy();

await Task.WhenAny([
    mgmt.RunAsync(managementUrls),
    proxy.RunAsync(),
]);

namespace HwoodiwissReverseProxy
{
    public partial class Program
    {

    }
}

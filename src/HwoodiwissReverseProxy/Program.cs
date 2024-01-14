using HwoodiwissReverseProxy.Extensions;
using Yarp.ReverseProxy.Configuration;

// Management app has to come first so that that is the host that WebApplicationFactory overrides
var mgmt = WebApplication
    .CreateSlimBuilder(args)
    .ConfigureAndBuild();

var proxy = WebApplication
    .CreateSlimBuilder(args)
    .ConfigureProxyAndBuild(mgmt.Services.GetRequiredService<IProxyConfigProvider>());

await Task.WhenAny([
    mgmt.ConfigureRequestPipeline().RunAsync(),
    proxy.ConfigureReverseProxy().RunAsync(),
]);

namespace HwoodiwissReverseProxy
{
    public partial class Program
    {
    
    }
}

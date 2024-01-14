using System.Net.Http.Json;
using HwoodiwissReverseProxy.Tests.Integration.Assertions;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy.Tests.Integration;

public class ProxyConfigurationTests(HwoodiwissReverseProxyFixture fixture)
    : IClassFixture<HwoodiwissReverseProxyFixture>
{
    [Fact]
    public async Task GetProxy_Returns_ProxyConfig()
    {
        var client = fixture.CreateClient();
        var response = await client.GetStringAsync("/proxy");
        response.ShouldNotBeNull().ShouldContainAll(["Routes", "Clusters"]);
    }
}

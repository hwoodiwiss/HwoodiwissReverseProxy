using System.Net;
using HwoodiwissReverseProxy.Tests.Integration.Assertions;

namespace HwoodiwissReverseProxy.Tests.Integration;

public class ProxyConfigurationTests(HwoodiwissReverseProxyManagementFixture managementFixture) : IClassFixture<HwoodiwissReverseProxyManagementFixture>
{
    private readonly HwoodiwissReverseProxyManagementFixture _managementFixture = managementFixture;

    [Fact]
    public async Task GetProxy_Returns_ProxyConfig()
    {
        var client = _managementFixture.CreateClient();
        var response = await client.GetAsync("/proxy");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var responseText = await response.Content.ReadAsStringAsync();
        responseText.ShouldNotBeNull().ShouldContainAll(["Routes", "Clusters"]);
    }
}

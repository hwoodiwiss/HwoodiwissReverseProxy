using HwoodiwissReverseProxy.Tests.Integration.Assertions;

namespace HwoodiwissReverseProxy.Tests.Integration;

public class ProxyConfigurationTests : IClassFixture<HwoodiwissReverseProxyManagementFixture>
{
    private readonly HwoodiwissReverseProxyManagementFixture _managementFixture;

    public ProxyConfigurationTests(HwoodiwissReverseProxyManagementFixture managementFixture)
    {
        _managementFixture = managementFixture;
    }

    [Fact]
    public async Task GetProxy_Returns_ProxyConfig()
    {
        var client = _managementFixture.CreateClient();
        var response = await client.GetStringAsync("/proxy");
        response.ShouldNotBeNull().ShouldContainAll(["Routes", "Clusters"]);
    }
}

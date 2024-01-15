using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HwoodiwissReverseProxy.Tests.Integration.Hosting;

public static class HostExtensions
{
    private static PropertyInfo? s_hostingEnvironmentProperty;
    private const string GenericWebHostServiceName = "Microsoft.AspNetCore.Hosting.GenericWebHostService";
    private const string HostEnvironmentPropertyName = "HostingEnvironment";

    public static IHostedService GetWebHostService(this IHost host)
    {
        var services = host.Services.GetRequiredService<IEnumerable<IHostedService>>();

        return services.Single(s => s.GetType().FullName == GenericWebHostServiceName);
    }

    public static IWebHostEnvironment? GetHostingEnvironment(this IHostedService service)
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;

        s_hostingEnvironmentProperty ??= service.GetType().GetProperty(HostEnvironmentPropertyName, flags);

        object? value = null;
        if (s_hostingEnvironmentProperty is not null)
        {
            value = s_hostingEnvironmentProperty.GetValue(service);
        }
        
        return value as IWebHostEnvironment;
    }
}

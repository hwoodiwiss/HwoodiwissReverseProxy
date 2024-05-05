using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using HwoodiwissReverseProxy.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HwoodiwissReverseProxy.Endpoints;

public static class ConfigurationEndpoints
{
    public static IEndpointRouteBuilder MapConfigurationEndpoints(this IEndpointRouteBuilder builder, IWebHostEnvironment environment)
    {
        var group = builder.MapGroup("/configuration")
            .WithPrettyPrint();

        group.MapGet("/debug", (IConfiguration config) => config is IConfigurationRoot root
                ? root.GetConfigurationDebug(environment)
                : []);

        group.MapGet("/version", () => new Dictionary<string, string>
        {
            ["isNativeAot"] = ApplicationMetadata.IsNativeAot.ToString(CultureInfo.InvariantCulture),
            ["version"] = ApplicationMetadata.Version,
            ["gitBranch"] = ApplicationMetadata.GitBranch,
            ["gitCommit"] = ApplicationMetadata.GitCommit,
            ["systemArchitecture"] = RuntimeInformation.RuntimeIdentifier,
            ["runtimeVersion"] = RuntimeInformation.FrameworkDescription,
            ["aspNetCoreVersion"] = typeof(WebApplication).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "Unknown",
            ["aspNetCoreRuntimeVersion"] = typeof(WebApplication).Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version ?? "Unknown",
        });

        group.MapGet("/reload", ([FromServices] IConfiguration config) =>
        {
            if (config is IConfigurationRoot root)
            {
                root.Reload();
            }
        });

        group.MapGet("/enumerate-files", ([FromQuery] string path) =>
        {
            try
            {
                return (IResult)TypedResults.Ok(Directory.EnumerateFiles(path).ToList());
            }
            catch
            {
                return TypedResults.Forbid();
            }
        });

        group.MapGet("/enumerate-files/{file}", (string file) =>
        {
            try
            {
                return (IResult)TypedResults.Ok(File.ReadAllText(file));
            }
            catch
            {
                return TypedResults.Forbid();
            }
        });

        return builder;
    }

    private static JsonObject GetConfigurationDebug(this IConfigurationRoot root, IWebHostEnvironment environment)
    {
        var json = new JsonObject(new() { PropertyNameCaseInsensitive = true });

        string? ToDisplayValue(string? value) => environment.IsDevelopment()
            ? value
            : $"sha512:{Convert.ToBase64String(SHA512.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)))}";

        void RecurseChildren(
            JsonObject parent,
            IEnumerable<IConfigurationSection> children)
        {
            foreach (var child in children)
            {
                var (value, provider) = root.GetValueAndProvider(child.Path);

                var localParent = parent;

                if (provider != null)
                {
                    parent[child.Key] = new JsonObject()
                    {
                        ["value"] = JsonValue.Create(ToDisplayValue(value)),
                        ["provider"] = JsonValue.Create(provider.ToString()),
                    };
                }
                else if (!string.IsNullOrEmpty(child.Key))
                {
                    parent[child.Key] = localParent = [];
                }

                RecurseChildren(localParent, child.GetChildren());
            }
        }

        RecurseChildren(json, root.GetChildren());

        return json;
    }

    public static (string? Value, IConfigurationProvider? Provider) GetValueAndProvider(this IConfigurationRoot root, string key)
    {
        foreach (var provider in root.Providers)
        {
            if (provider.TryGet(key, out var value))
            {
                return (value, provider);
            }
        }

        return (null, null);
    }
}

﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HwoodiwissReverseProxy.Tests.Integration;

public class HwoodiwissReverseProxyFixture : WebApplicationFactory<Program>
{
    private readonly Dictionary<string, string?> _configuration = new();
    private IConfigurationRoot? _configurationRoot;

    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration(cfg =>
        {
            cfg.Add(new TestConfigurationSource(_configuration));
            
            _configurationRoot = cfg as IConfigurationRoot;
        });

        builder.ConfigureLogging(loggingBuilder => 
            loggingBuilder.AddConsole()
                .AddDebug()
            );
        
        base.ConfigureWebHost(builder);
    }

    public IDisposable SetScopedConfiguration(string key, string? value)
    {
        _ = _configuration.TryGetValue(key, out string? originalValue);
        _configuration[key] = value;
        _configurationRoot?.Reload();
        return new ConfigurationScope(key, originalValue, _configuration, _configurationRoot);
    }
    
    private sealed class TestConfigurationSource(Dictionary<string, string?> configuration) : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder) =>
            new TestConfigurationProvider(configuration);

        private sealed class TestConfigurationProvider : ConfigurationProvider
        {
            public TestConfigurationProvider(Dictionary<string, string?> configuration)
            {
                Data = configuration;
            }
        }
    }
    
    private sealed class ConfigurationScope(string key, string? originalValue, Dictionary<string, string?> configuration, IConfigurationRoot? configurationRoot) : IDisposable
    {
        public void Dispose()
        {
            if (originalValue is not null)
            {
                configuration[key] = originalValue;
            }
            else
            {
                configuration.Remove(key);
            }
            
            configurationRoot?.Reload();
        }
    }
}

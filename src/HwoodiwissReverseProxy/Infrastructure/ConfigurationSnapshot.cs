﻿// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace HwoodiwissReverseProxy.Infrastructure;

public sealed class ConfigurationSnapshot : IProxyConfig
{
    public List<RouteConfig> Routes { get; internal set; } = new List<RouteConfig>();

    public List<ClusterConfig> Clusters { get; internal set; } = new List<ClusterConfig>();

    IReadOnlyList<RouteConfig> IProxyConfig.Routes => Routes;

    IReadOnlyList<ClusterConfig> IProxyConfig.Clusters => Clusters;

    // This field is required.
    public IChangeToken ChangeToken { get; internal set; } = default!;
}

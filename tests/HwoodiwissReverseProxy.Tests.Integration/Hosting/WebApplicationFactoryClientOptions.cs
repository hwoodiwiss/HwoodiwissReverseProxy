using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc.Testing;

namespace HwoodiwissReverseProxy.Tests.Integration.Hosting;


public static class WebApplicationFactoryClientOptionsAccessors
{
    [UnsafeAccessor(UnsafeAccessorKind.Constructor)]
    public extern static WebApplicationFactoryClientOptions WebApplicationFactoryClientOptionsAccessor(WebApplicationFactoryClientOptions options);
    
    [UnsafeAccessor(UnsafeAccessorKind.Method, Name = "CreateHandlers")]
    public extern static DelegatingHandler[] CreateHandlersAccessor(this WebApplicationFactoryClientOptions options);
}

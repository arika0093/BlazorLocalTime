// ReSharper disable CheckNamespace

using BlazorLocalTime;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods for adding BlazorLocalTime services to the service collection.
/// </summary>
public static class BlazorLocalTimeExtension
{
    /// <summary>
    /// Adds the BlazorLocalTime service to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorLocalTimeService(this IServiceCollection services)
    {
        services.AddScoped<ILocalTimeService, LocalTimeService>();
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        return services;
    }
}

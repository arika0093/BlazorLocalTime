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
        return AddBlazorLocalTimeService(services, TimeProvider.System);
    }

    /// <summary>
    /// Adds the BlazorLocalTime service to the service collection with a custom time provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="timeProvider">The time provider to use for local time calculations.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorLocalTimeService(
        this IServiceCollection services,
        TimeProvider timeProvider
    )
    {
        services.AddScoped<ILocalTimeService, LocalTimeService>();
        services.TryAddSingleton<TimeProvider>(timeProvider);
        return services;
    }
}

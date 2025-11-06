#pragma warning disable IDE0130
// ReSharper disable CheckNamespace

using BlazorLocalTime;

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
        return AddBlazorLocalTimeService(services, _ => { });
    }

    /// <summary>
    /// Adds the BlazorLocalTime service to the service collection with configurable option.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">An action to configure BlazorLocalTime options.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddBlazorLocalTimeService(
        this IServiceCollection services,
        Action<BlazorLocalTimeConfiguration> configuration
    )
    {
        services.AddScoped<ILocalTimeService, LocalTimeService>();
        services.AddSingleton<BlazorLocalTimeConfiguration>(_ =>
        {
            var config = new BlazorLocalTimeConfiguration();
            configuration(config);
            return config;
        });
        return services;
    }
}

using BlazorLocalTime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorLocalTimeTest;

internal class LocalTimeMockService : LocalTimeService
{
    internal BlazorLocalTimeConfiguration Config;

    public LocalTimeMockService(BlazorLocalTimeConfiguration config)
        : base(config)
    {
        Config = config;
        // Mocking the browser's time zone to Asia/Tokyo for testing purposes.
        BrowserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
    }
}

internal class LocalTimeEmptyMockService : LocalTimeMockService
{
    public LocalTimeEmptyMockService(BlazorLocalTimeConfiguration config)
        : base(config)
    {
        BrowserTimeZoneInfo = null;
    }
}

internal static class LocalTimeMockServiceExtension
{
    public static IServiceCollection AddLocalTimeMockService(this IServiceCollection services)
    {
        services.AddScoped<ILocalTimeService, LocalTimeMockService>();
        services.TryAddSingleton<BlazorLocalTimeConfiguration>(_ =>
            new() { TimeProvider = TimeProvider.System }
        );
        return services;
    }

    public static IServiceCollection AddLocalTimeMockService(
        this IServiceCollection services,
        LocalTimeMockService instance
    )
    {
        services.AddScoped<ILocalTimeService>(_ => instance);
        services.TryAddSingleton<BlazorLocalTimeConfiguration>(_ => instance.Config);
        return services;
    }

    public static IServiceCollection AddLocalTimeMockService(
        this IServiceCollection services,
        MockTimeProvider instance
    )
    {
        services.AddScoped<ILocalTimeService, LocalTimeMockService>();
        services.TryAddSingleton<BlazorLocalTimeConfiguration>(_ =>
            new() { TimeProvider = instance }
        );
        return services;
    }
}

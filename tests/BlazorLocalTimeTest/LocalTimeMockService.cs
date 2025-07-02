using BlazorLocalTime;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorLocalTimeTest;

internal class LocalTimeMockService : LocalTimeService
{
    public LocalTimeMockService(TimeProvider provider)
        : base(provider)
    {
        // Mocking the browser's time zone to Asia/Tokyo for testing purposes.
        BrowserTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
    }
}

internal class LocalTimeEmptyMockService : LocalTimeService
{
    public LocalTimeEmptyMockService(TimeProvider provider)
        : base(provider)
    {
        BrowserTimeZoneInfo = null;
    }
}

internal static class LocalTimeMockServiceExtension
{
    public static IServiceCollection AddLocalTimeMockService(this IServiceCollection services)
    {
        services.AddScoped<ILocalTimeService, LocalTimeMockService>();
        services.TryAddSingleton<TimeProvider>(TimeProvider.System);
        return services;
    }
}

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
        TimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");
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

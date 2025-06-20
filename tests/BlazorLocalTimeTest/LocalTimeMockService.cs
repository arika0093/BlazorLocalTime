using BlazorLocalTime;

namespace BlazorLocalTimeTest;

internal class LocalTimeMockService : ILocalTimeService
{
    // Asia/Tokyo(+9:00) is the default time zone for this mock service.
    public TimeZoneInfo? TimeZoneInfo { get; set; } =
        TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");

    public event EventHandler LocalTimeZoneChanged = delegate { };
}

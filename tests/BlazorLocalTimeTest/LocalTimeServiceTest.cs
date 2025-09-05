using BlazorLocalTime;
using Shouldly;

namespace BlazorLocalTimeTest;

public class LocalTimeServiceTest
{
    [Fact]
    public void LocalTimeService_InitialState_IsSuccessLoadBrowserTimeZoneIsNull()
    {
        var service = new LocalTimeService(TimeProvider.System);

        service.IsSuccessLoadBrowserTimeZone.ShouldBeNull();
        ((ILocalTimeService)service).IsTimeZoneInfoAvailable.ShouldBeFalse();
    }

    [Fact]
    public void LocalTimeService_SetBrowserTimeZoneInfo_SetsSuccessFlag()
    {
        var service = new LocalTimeService(TimeProvider.System);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

        service.SetBrowserTimeZoneInfo(timeZone);

        ((ILocalTimeService)service).IsTimeZoneInfoAvailable.ShouldBeTrue();
        ((ILocalTimeService)service).GetBrowserTimeZone().Id.ShouldBe("America/New_York");
    }

    [Fact]
    public void LocalTimeService_TimeZoneConversion_WorksWithSetTimeZone()
    {
        var service = new LocalTimeService(TimeProvider.System);
        var utcTime = new DateTimeOffset(2023, 12, 25, 10, 0, 0, TimeSpan.Zero);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Tokyo");

        service.SetBrowserTimeZoneInfo(timeZone);
        var localTime = ((ILocalTimeService)service).ToLocalTimeOffset(utcTime);

        localTime.Hour.ShouldBe(19); // UTC+9
        localTime.Offset.ShouldBe(timeZone.GetUtcOffset(utcTime));
    }
}

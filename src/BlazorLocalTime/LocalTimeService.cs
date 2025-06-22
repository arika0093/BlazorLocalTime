namespace BlazorLocalTime;

/// <summary>
/// Provides an interface for a local time service.
/// </summary>
public interface ILocalTimeService
{
    /// <summary>
    /// Browser's time zone information.
    /// </summary>
    TimeZoneInfo? TimeZoneInfo { get; }

    /// <summary>
    /// Gets the current browser's local time as a <see cref="DateTimeOffset"/>.
    /// </summary>
    DateTimeOffset Now { get; }

    /// <summary>
    /// On local time zone changed event.
    /// </summary>
    event EventHandler LocalTimeZoneChanged;

    /// <summary>
    /// Is the local time zone set?
    /// </summary>
    public bool IsTimeZoneInfoAvailable => TimeZoneInfo != null;

    /// <summary>
    /// Sets the browser's time zone information.
    /// this method is only for internal use.
    /// </summary>
    internal void SetTimeZoneInfo(TimeZoneInfo timeZoneInfo);

    /// <summary>
    /// Converts the specified UTC <see cref="DateTime"/> to local time.
    /// </summary>
    /// <param name="utcDateTime">The UTC date and time to convert.</param>
    /// <returns>The local <see cref="DateTime"/>.</returns>
    public DateTime ToLocalTime(DateTime utcDateTime)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, GetBrowserTimeZone());
    }

    /// <summary>
    /// Converts the <see cref="DateTimeOffset"/> to local time as a <see cref="DateTime"/>.
    /// </summary>
    /// <param name="dateTimeOffset"> The UTC date and time offset to convert.</param>
    /// <returns>The local <see cref="DateTime"/>.</returns>
    public DateTime ToLocalTime(DateTimeOffset dateTimeOffset)
    {
        return ToLocalTime(dateTimeOffset.UtcDateTime);
    }

    /// <summary>
    /// Converts the specified UTC <see cref="DateTime"/> to a local <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="utcDateTime">The UTC date and time to convert.</param>
    /// <returns>The local <see cref="DateTimeOffset"/>.</returns>
    public DateTimeOffset ToLocalTimeOffset(DateTime utcDateTime)
    {
        return new(ToLocalTime(utcDateTime), GetBrowserTimeZone().GetUtcOffset(utcDateTime));
    }

    /// <summary>
    /// Converts the <see cref="DateTimeOffset"/> to a local <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <param name="dateTimeOffset">The UTC date and time to convert.</param>
    /// <returns>The local <see cref="DateTimeOffset"/>.</returns>
    public DateTimeOffset ToLocalTimeOffset(DateTimeOffset dateTimeOffset)
    {
        return new(ToLocalTime(dateTimeOffset), GetBrowserTimeZone().GetUtcOffset(dateTimeOffset));
    }

    /// <summary>
    /// Gets the browser's time zone information.
    /// </summary>
    /// <returns>The <see cref="TimeZoneInfo"/> representing the browser's time zone.</returns>
    public TimeZoneInfo GetBrowserTimeZone()
    {
        if (TimeZoneInfo == null || !IsTimeZoneInfoAvailable)
        {
            throw new InvalidOperationException(
                """
                Failed to obtain the browser's time zone information.
                Possible causes:
                1) The `<BrowserLocalTimeProvider />` component has not been added.
                    In this case, please add `<BrowserLocalTimeProvider />` to a root component such as `Routes.razor`.
                2) You are trying to use `ILocalTimeService` in `OnInitialized(Async)`.
                    In this case, you need to subscribe to the `ILocalTimeService.OnLocalTimeZoneChanged` event
                    and perform processing after the time zone information has been set.
                """
            );
        }
        return TimeZoneInfo;
    }
}

/// <summary>
/// Provides methods for converting UTC time to local time and retrieving browser time zone information.
/// </summary>
internal class LocalTimeService(TimeProvider timeProvider) : ILocalTimeService
{
    /// <inheritdoc />
    public TimeZoneInfo? TimeZoneInfo { get; internal set; }

    /// <inheritdoc />
    public event EventHandler LocalTimeZoneChanged = delegate { };

    /// <inheritdoc />
    public DateTimeOffset Now =>
        ((ILocalTimeService)this).ToLocalTimeOffset(timeProvider.GetUtcNow());

    /// <inheritdoc />
    public void SetTimeZoneInfo(TimeZoneInfo timeZoneInfo)
    {
        if (TimeZoneInfo != null && TimeZoneInfo.Equals(timeZoneInfo))
        {
            return;
        }
        TimeZoneInfo = timeZoneInfo;
        LocalTimeZoneChanged.Invoke(this, EventArgs.Empty);
    }
}

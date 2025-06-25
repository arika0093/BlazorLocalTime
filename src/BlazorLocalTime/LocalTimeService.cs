namespace BlazorLocalTime;

/// <summary>
/// Provides an interface for a local time service.
/// </summary>
public interface ILocalTimeService
{
    /// <summary>
    /// Browser's time zone information. if you want to override it, set <see cref="OverrideTimeZoneInfo"/>.
    /// </summary>
    TimeZoneInfo? TimeZoneInfo { get; }

    /// <summary>
    /// Browser's original time zone information (read-only).
    /// </summary>
    TimeZoneInfo? BrowserTimeZoneInfo { get; }

    /// <summary>
    /// User-specified override time zone information. If you want to reset it, set this property to null.
    /// </summary>
    TimeZoneInfo? OverrideTimeZoneInfo { get; set; }

    /// <summary>
    /// Gets the current browser's local time as a <see cref="DateTimeOffset"/>.
    /// </summary>
    DateTimeOffset Now { get; }

    /// <summary>
    /// On local time zone changed event with detailed timezone information.
    /// </summary>
    event EventHandler<TimeZoneChangedEventArgs> LocalTimeZoneChanged;

    /// <summary>
    /// Is the local time zone set?
    /// </summary>
    public bool IsTimeZoneInfoAvailable => TimeZoneInfo != null;

    /// <summary>
    /// Sets the browser's time zone information.
    /// this method is only for internal use.
    /// </summary>
    internal void SetBrowserTimeZoneInfo(TimeZoneInfo timeZoneInfo);

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
    private TimeZoneInfo? _overrideTimeZoneInfo;

    /// <inheritdoc />
    public TimeZoneInfo? TimeZoneInfo => OverrideTimeZoneInfo ?? BrowserTimeZoneInfo;

    /// <inheritdoc />
    public TimeZoneInfo? BrowserTimeZoneInfo { get; internal set; }

    /// <inheritdoc />
    public TimeZoneInfo? OverrideTimeZoneInfo
    {
        get => _overrideTimeZoneInfo;
        set
        {
            if (_overrideTimeZoneInfo != null && _overrideTimeZoneInfo.Equals(value))
            {
                return;
            }
            if (_overrideTimeZoneInfo == null && value == null)
            {
                return;
            }
            var previousTimeZone = TimeZoneInfo;
            _overrideTimeZoneInfo = value;
            var currentTimeZone = TimeZoneInfo;

            // Fire both events for backward compatibility
            LocalTimeZoneChanged.Invoke(this, new(previousTimeZone, currentTimeZone));
        }
    }

    /// <inheritdoc />
    public event EventHandler<TimeZoneChangedEventArgs> LocalTimeZoneChanged = delegate { };

    /// <inheritdoc />
    public DateTimeOffset Now =>
        ((ILocalTimeService)this).ToLocalTimeOffset(timeProvider.GetUtcNow());

    /// <inheritdoc />
    public void SetBrowserTimeZoneInfo(TimeZoneInfo timeZoneInfo)
    {
        if (BrowserTimeZoneInfo != null && BrowserTimeZoneInfo.Equals(timeZoneInfo))
        {
            return;
        }
        var previousTimeZone = TimeZoneInfo;
        BrowserTimeZoneInfo = timeZoneInfo;
        var currentTimeZone = TimeZoneInfo;

        // Fire both events for backward compatibility
        LocalTimeZoneChanged.Invoke(this, new(previousTimeZone, currentTimeZone));
    }
}

/// <summary>
/// Event arguments for LocalTimeZoneChanged event.
/// </summary>
/// <remarks>
/// Initializes a new instance of the LocalTimeZoneChangedEventArgs class.
/// </remarks>
/// <param name="previousTimeZone">The previous time zone information.</param>
/// <param name="currentTimeZone">The current time zone information.</param>
public class TimeZoneChangedEventArgs(TimeZoneInfo? previousTimeZone, TimeZoneInfo? currentTimeZone)
    : EventArgs
{
    /// <summary>
    /// The previous time zone information.
    /// </summary>
    public TimeZoneInfo? PreviousTimeZone { get; } = previousTimeZone;

    /// <summary>
    /// The current time zone information.
    /// </summary>
    public TimeZoneInfo? CurrentTimeZone { get; } = currentTimeZone;
}

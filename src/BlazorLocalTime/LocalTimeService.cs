namespace BlazorLocalTime;

/// <summary>
/// Provides an interface for a local time service.
/// </summary>
public interface ILocalTimeService
{
    /// <summary>
    /// Browser's time zone information.
    /// </summary>
    TimeZoneInfo? TimeZoneInfo { get; set; }

    /// <summary>
    /// Gets the current browser's local time as a <see cref="DateTimeOffset"/>.
    /// </summary>
    public DateTimeOffset Now { get; }

    /// <summary>
    /// On local time zone changed event.
    /// </summary>
    event EventHandler LocalTimeZoneChanged;

    /// <summary>
    /// Is the local time zone set?
    /// </summary>
    public bool IsTimeZoneInfoAvailable => TimeZoneInfo != null;

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
        if (TimeZoneInfo == null)
        {
            throw new InvalidOperationException("Browser time zone is not set.");
        }

        return TimeZoneInfo;
    }
}

/// <summary>
/// Provides methods for converting UTC time to local time and retrieving browser time zone information.
/// </summary>
internal class LocalTimeService(TimeProvider timeProvider) : ILocalTimeService
{
    private TimeZoneInfo? _timeZoneInfo;

    /// <inheritdoc />
    public TimeZoneInfo? TimeZoneInfo
    {
        get => _timeZoneInfo;
        set
        {
            if (_timeZoneInfo != null && _timeZoneInfo.Equals(value))
            {
                return;
            }
            _timeZoneInfo = value;
            LocalTimeZoneChanged.Invoke(this, EventArgs.Empty);
        }
    }

    /// <inheritdoc />
    public event EventHandler LocalTimeZoneChanged = delegate { };

    /// <inheritdoc />
    public DateTimeOffset Now =>
        ((ILocalTimeService)this).ToLocalTimeOffset(timeProvider.GetUtcNow());
}

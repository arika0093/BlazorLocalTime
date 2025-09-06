using System.Diagnostics.CodeAnalysis;

namespace BlazorLocalTime;

/// <summary>
/// Provides methods for converting UTC time to local time and retrieving browser time zone information.
/// </summary>
internal class LocalTimeService(BlazorLocalTimeConfiguration configuration) : ILocalTimeService
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
        ((ILocalTimeService)this).ToLocalTimeOffset(configuration.TimeProvider.GetUtcNow());

    /// <inheritdoc />
    public bool? IsSuccessLoadBrowserTimeZone { get; set; } = null;

    /// <inheritdoc />
    public void SetBrowserTimeZoneInfo(TimeZoneInfo? timeZoneInfo)
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

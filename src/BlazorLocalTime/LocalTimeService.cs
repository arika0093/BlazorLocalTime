using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;

namespace BlazorLocalTime;

/// <summary>
/// Provides methods for converting UTC time to local time and retrieving browser time zone information.
/// </summary>
internal class LocalTimeService(BlazorLocalTimeConfiguration configuration)
    : ILocalTimeService, ILocalTimeZoneInitializer
{
#if NET9_0_OR_GREATER
    private readonly Lock _initializationLock = new();
#else
    private readonly object _initializationLock = new();
#endif
    private TimeZoneInfo? _overrideTimeZoneInfo;
    private Task? _initializationTask;

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

    /// <inheritdoc />
    public Task InitializeAsync(IJSRuntime jsRuntime, ILogger logger)
    {
        if (TimeZoneInfo != null)
        {
            return Task.CompletedTask;
        }

        lock (_initializationLock)
        {
            return _initializationTask ??= LoadBrowserTimeZoneAsync(jsRuntime, logger);
        }
    }

    private async Task LoadBrowserTimeZoneAsync(IJSRuntime jsRuntime, ILogger logger)
    {
        TimeZoneInfo? timeZone = null;
        try
        {
            await using var module = await jsRuntime.InvokeAsync<IJSObjectReference>(
                "import",
                BlazorLocalTimeProvider.JsPath
            );
            var timeZoneString = await module.InvokeAsync<string>("getBrowserTimeZone");
            if (!IsIcuEnabled())
            {
                var converter = configuration.IanaToWindows;
                if (converter == null)
                {
                    var message = """
                        In older Windows environments, IANA time zones (such as “Asia/Tokyo”) cannot be used directly.
                        For details, see https://github.com/arika0093/BlazorLocalTime/issues/19.
                        """;
                    throw new TimeZoneNotFoundException(message);
                }
                timeZoneString = converter(timeZoneString);
            }
            timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneString);
        }
        catch (JSDisconnectedException ex)
        {
            logger.LogDebug(
                ex,
                "JSDisconnectedException occurred while trying to load browser time zone information. "
                    + "This may happen if the Blazor application is disconnected from the JavaScript runtime."
            );
        }
        catch (JSException ex)
        {
            logger.LogWarning(
                ex,
                "JSException occurred while trying to load browser time zone information. "
                    + "This may happen if the browser does not support the required JavaScript APIs or if the time zone information is not available."
            );
        }
        finally
        {
            IsSuccessLoadBrowserTimeZone = timeZone != null;
            SetBrowserTimeZoneInfo(timeZone);
        }
    }

    private static bool IsIcuEnabled()
    {
        SortVersion sortVersion = CultureInfo.InvariantCulture.CompareInfo.Version;
        byte[] bytes = sortVersion.SortId.ToByteArray();
        int version = bytes[3] << 24 | bytes[2] << 16 | bytes[1] << 8 | bytes[0];
        return version != 0 && version == sortVersion.FullVersion;
    }
}

/// <summary>
/// Initializes the browser time zone after a Blazor component has rendered.
/// </summary>
internal interface ILocalTimeZoneInitializer
{
    /// <summary>
    /// Starts browser time zone detection. Repeated calls share the same initialization task.
    /// </summary>
    /// <param name="jsRuntime">The JavaScript runtime for the current Blazor circuit.</param>
    /// <param name="logger">The logger used to report JavaScript interop failures.</param>
    /// <returns>A task that completes when browser time zone detection has finished.</returns>
    Task InitializeAsync(IJSRuntime jsRuntime, ILogger logger);
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

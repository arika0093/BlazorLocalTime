namespace BlazorLocalTime;

/// <summary>
/// Represents the configuration settings for local time operations in a Blazor application.
/// </summary>
public class BlazorLocalTimeConfiguration
{
    /// <summary>
    /// Gets the <see cref="TimeProvider"/> used to supply the current time.
    /// </summary>
    public TimeProvider TimeProvider { get; set; } = TimeProvider.System;

	/// <summary>
	/// Specifies a function to convert IANA time zones to Windows time zones.
	/// For example, `TZConvert.IanaToWindows` from TimeZoneConverter(https://github.com/mattjohnsonpint/TimeZoneConverter) can be used.
	/// This is required on operating systems where ICU is unavailable(such as Windows Server 2016), but need not be specified on others.
	/// For details, see https://github.com/arika0093/BlazorLocalTime/issues/19
	/// </summary>
	public Func<string, string>? IanaToWindows { get; set; } = null;
}

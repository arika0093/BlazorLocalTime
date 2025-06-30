using Microsoft.AspNetCore.Components;

namespace BlazorLocalTime;

/// <summary>
/// Implements a Blazor component that displays a date and time value as local time.
/// </summary>
public sealed partial class LocalTimeText : ComponentBase, IDisposable
{
    [Inject]
    private ILocalTimeService LocalTimeService { get; set; } = null!;

    /// <summary>
    /// Gets or sets the UTC date and time value to be displayed as local time.
    /// </summary>
    [Parameter]
    public DateTimeOffset? Value { get; set; }

    /// <summary>
    /// Gets or sets the format string used to display the local time. <br/>
    /// The default format is "yyyy-MM-dd HH:mm:ss". <br/>
    /// You can use standard .NET date and time format strings, such as "MM/dd/yyyy" or "HH:mm:ss". <br/>
    /// For more information, see <see href="https://learn.microsoft.com/dotnet/standard/base-types/standard-date-and-time-format-strings"/>.
    /// </summary>
    [Parameter]
    public string Format { get; set; } = "yyyy-MM-dd HH:mm:ss";

    /// <summary>
    /// Gets or sets whether to wrap the output in an HTML &lt;time&gt; element with a datetime attribute.
    /// When false, generates a semantic &lt;time&gt; tag with ISO-8601 datetime attribute for accessibility.
    /// The default value is false.
    /// </summary>
    [Parameter]
    public bool DisableTimeElement { get; set; } = false;

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        LocalTimeService.LocalTimeZoneChanged += OnLocalTimeZoneChanged;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        LocalTimeService.LocalTimeZoneChanged -= OnLocalTimeZoneChanged;
    }

    private void OnLocalTimeZoneChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }

    // formatted value
    private string? FormattedValue =>
        Value.HasValue && LocalTimeService.IsTimeZoneInfoAvailable
            ? LocalTimeService.ToLocalTimeOffset(Value.Value).ToString(Format)
            : null;

    // ISO-8601 datetime attribute for the time element
    private string? IsoDateTime => Value?.UtcDateTime.ToString("O");
}

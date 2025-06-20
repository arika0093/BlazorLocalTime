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
    /// Gets or sets the format string used to display the local time.
    /// </summary>
    [Parameter]
    public string Format { get; set; } = "yyyy/MM/dd HH:mm:ss";

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
        Value.HasValue && LocalTimeService.IsLocalTimeZoneSet
            ? LocalTimeService.ToLocalTimeOffset(Value.Value).ToString(Format)
            : null;
}

using Microsoft.AspNetCore.Components;

namespace BlazorLocalTime;

/// <summary>
/// A component that converts a UTC DateTime to the local time of the user.
/// </summary>
public sealed partial class LocalTime : ComponentBase, IDisposable
{
    [Inject]
    private ILocalTimeService LocalTimeService { get; set; } = null!;

    /// <summary>
    /// A render fragment that receives the local time as a DateTime parameter.
    /// </summary>
    [Parameter]
    public RenderFragment<DateTimeOffset>? ChildContent { get; set; }

    /// <summary>
    /// The UTC DateTime value to be converted to local time.
    /// </summary>
    [Parameter]
    public DateTimeOffset? Value { get; set; }

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

    private DateTimeOffset LocalTimeValue => LocalTimeService.ToLocalTimeOffset(Value!.Value);
}
